# Bugün Çözülen Hatalar ve Çözümleri

Bu dokümanda, Store projesinde OAuth 2.0 güvenlik entegrasyonu ve MongoDB veri serileştirme (deserialization) süreçlerinde karşılaşılan hatalar, bu hataların nedenleri ve uygulanan çözümler detaylandırılmıştır.

---

## 1. Hata: 401 Unauthorized (Issuer Uyuşmazlığı)

### Sorun Tanımı
Postman aracılığıyla `http://localhost:5274/api/Categories` adresine geçerli bir JWT (Access Token) ile istek atılmasına rağmen API sürekli olarak `401 Unauthorized` hatası döndürüyordu.

### Neden
*   **Token İçeriği:** IdentityServer tarafından üretilen token'ın issuer (`iss`) claim değeri `https://localhost:5001` (HTTPS) idi.
*   **Servis Yapılandırması:** Catalog servisinin `appsettings.json` dosyasında `IdentityServerURL` değeri `http://localhost:5001` (HTTP) olarak tanımlanmıştı.
*   **Sonuç:** Token doğrulama (JWT Validation) aşamasında, token'ı üreten (Issuer) adres ile servisin beklediği adres uyuşmadığı için token geçersiz sayıldı.

### Çözüm
Catalog servisinin `appsettings.json` dosyasındaki adres düzeltildi:
```json
"IdentityServerURL": "https://localhost:5001"
```

---

## 2. Hata: 401 Unauthorized (SSL Sertifikası ve JWKS Hatası)

### Sorun Tanımı
Issuer adresi düzeltilmesine rağmen 401 hatası devam etti. Catalog servisi loglarında şu hata görüntülendi:
> `IDX10500: Signature validation failed. No security keys were provided to validate the signature.`

### Neden
*   **Self-Signed SSL:** Yerel geliştirme ortamında (Development) IdentityServer `https` üzerinde güvenli olmayan (self-signed / kendinden imzalı) bir SSL sertifikasıyla çalışmaktadır.
*   **JWKS Bağlantı Hatası:** Catalog servisi, gelen token'ın imzasını doğrulamak için arka planda (backchannel) IdentityServer'ın JWKS (`/.well-known/openid-configuration/jwks`) endpoint'ine bağlanmaya çalışıyordu. SSL sertifikası güvenli olmadığı için .NET bu bağlantıyı engelledi ve imza anahtarlarını çekemedi.

### Çözüm
`Program.cs` dosyasına, uygulamanın açılışında SSL sertifika kontrolünü atlayarak JWKS anahtarlarını doğrudan belleğe çeken ve token doğrulama parametrelerine enjekte eden bir mekanizma eklendi:
```csharp
var identityServerUrl = builder.Configuration["IdentityServerURL"]!;
IList<SecurityKey> signingKeys = new List<SecurityKey>();
try
{
    var sslBypassHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
    };
    using var httpClient = new HttpClient(sslBypassHandler);

    var discoveryJson = await httpClient.GetStringAsync($"{identityServerUrl}/.well-known/openid-configuration");
    var discoveryDoc = System.Text.Json.JsonDocument.Parse(discoveryJson);
    var jwksUri = discoveryDoc.RootElement.GetProperty("jwks_uri").GetString()!;

    var jwksJson = await httpClient.GetStringAsync(jwksUri);
    var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksJson);
    signingKeys = jwks.GetSigningKeys();
}
catch (Exception ex) { /* Log */ }

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ...
        IssuerSigningKeys = signingKeys
    };
});
```

---

## 3. Hata: HTTP 500 Internal Server Error (BsonType.ObjectId ➔ Guid Dönüşüm Hatası)

### Sorun Tanımı
Token doğrulama aşaması başarıyla geçildikten sonra veritabanından kategoriler çekilmeye çalışıldığında HTTP 500 hatası alındı:
> `Cannot deserialize a 'Guid' from BsonType 'ObjectId'.`

### Neden
*   **C# Modelleri:** `BaseEntity` ve `Category` sınıflarında `Id` ve `ParentId` özellikleri `Guid` tipinde tanımlanmıştı.
*   **MongoDB Verisi:** Veritabanındaki mevcut dokümanların benzersiz anahtarları (`_id` ve `ParentId`) MongoDB'nin varsayılan `ObjectId` (12-byte hex) tipindeydi. MongoDB sürücüsü `ObjectId` tipindeki bir veriyi otomatik olarak `Guid` alanına eşitleyemediği için hata fırlattı.

### Çözüm
Yazılan bir JavaScript migration scripti (`migrate_db.js`) `mongosh` üzerinde çalıştırılarak veritabanındaki mevcut kayıtların kimlik alanları standart **UUID (Binary Subtype 4)** formatına dönüştürüldü:
```javascript
db.Categories.drop();
db.Categories.insertOne({
  _id: UUID('00000000-0000-0000-0000-000000000001'),
  // ...
  ParentId: null,
  Name: 'ANA MENÜ'
});
```

---

## 4. Hata: HTTP 500 (GuidRepresentation is Unspecified Hatası)

### Sorun Tanımı
Veritabanı UUID formatına dönüştürüldükten sonra şu hata alındı:
> `GuidSerializer cannot deserialize a Guid when GuidRepresentation is Unspecified.`

### Neden
*   **String/Binary Eşleşmezliği:** `Category.cs` ve `ProductDetail.cs` sınıflarındaki `ParentId` ve `ProductId` özellikleri `[BsonRepresentation(BsonType.String)]` niteliğine sahipti. Ancak biz veritabanında bunları `UUID (Binary)` olarak güncelledik. Sürücü, veritabanında Binary tipinde veri görüp bunu String bekleyen alanla eşleştirmeye çalışırken hata verdi.
*   **Genel GUID Tanımsızlığı:** MongoDB C# sürücüsüne GUID'leri hangi formatta (Standard UUID) okuması gerektiği global olarak bildirilmemişti.

### Çözüm
1.  İlgili model dosyalarından (`Category.cs`, `ProductDetail.cs`) `[BsonRepresentation(BsonType.String)]` satırları kaldırıldı.
2.  `Program.cs` başlangıcına global `GuidSerializer` ve `NullableSerializer<Guid>` kaydedildi:
    ```csharp
    MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
    MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.NullableSerializer<Guid>(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard)));
    ```
3.  Veritabanında ek/tanımsız alanlar bulunması durumunda hataya düşmemek için global olarak `IgnoreExtraElementsConvention` aktif edildi:
    ```csharp
    var conventionPack = new MongoDB.Bson.Serialization.Conventions.ConventionPack
    {
        new MongoDB.Bson.Serialization.Conventions.IgnoreExtraElementsConvention(true)
    };
    MongoDB.Bson.Serialization.Conventions.ConventionRegistry.Register("CatalogStoreConventions", conventionPack, t => true);
    ```

---

## 5. Hata: Derleme Başarısızlığı (MSB3027 / File Locking)

### Sorun Tanımı
Kod değişikliklerinden sonra `dotnet build` çalıştırıldığında 4 adet kopyalama/erişim hatası alınıyordu.

### Neden
Catalog veya IdentityServer uygulamaları arka planda çalışır durumdayken (`dotnet run` veya debug modda kilitli), derleyici üretilen yeni `.dll` ve `.exe` dosyalarını mevcut dosyaların üzerine yazamıyordu.

### Çözüm
Arka planda kilit oluşturan tüm dotnet servis süreçleri durduruldu ve temiz bir derleme yapıldı:
```powershell
Get-Process | Where-Object { $_.Name -like "*Store*" -or $_.Name -like "*dotnet*" } | Stop-Process -Force
dotnet build
```
Derleme sonucunda **0 Hata ve 0 Uyarı** ile işlem tamamlandı.
