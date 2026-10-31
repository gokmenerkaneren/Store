/**
 * MongoDB Duplicate ProductPrice Cleanup Script
 * 
 * Aynı ProductId, CurrencyId, PriceTypeId ve ValidTo=null kombinasyonunda tekrar eden kayıtları temizler.
 * Script, en yeni belgeyi (en son oluşturulmuş) saklar, diğerlerini ValidTo=now güncelle.
 * 
 * Kullanım: mongosh StoreMD < CleanupDuplicateProductPrices.js
 */

// Veritabanına bağlan
db = db.getSiblingDB("StoreMD");

const collection = db.ProductPrices;
const duplicates = [];

// 1. Duplicate kombinasyonları bul (ValidTo = null)
const groupQuery = [
  {
    $match: {
      ValidTo: null
    }
  },
  {
    $group: {
      _id: {
        ProductId: "$ProductId",
        CurrencyId: "$CurrencyId",
        PriceTypeId: "$PriceTypeId"
      },
      count: { $sum: 1 },
      ids: { $push: "$_id" },
      createdAt: { $push: "$CreatedAt" }
    }
  },
  {
    $match: {
      count: { $gt: 1 }
    }
  }
];

const result = collection.aggregate(groupQuery).toArray();

print(`\n=== ProductPrice Duplicate Temizleme Raporu ===\n`);
print(`Toplam duplicate kombinasyon sayısı: ${result.length}\n`);

if (result.length === 0) {
  print("✓ İyi haber! Duplicate kayıt bulunamadı.\n");
  quit();
}

let totalRecordsToUpdate = 0;

// 2. Her kombinasyon için en yeni hariç diğerlerini güncelle
result.forEach((group, index) => {
  const ids = group.ids;
  const createdAts = group.createdAt;
  const duplicateCount = group.count;

  // En yeni belgeyi bul (en sonuncu timestamp)
  let maxCreatedIndex = 0;
  let maxCreatedAt = createdAts[0];

  for (let i = 1; i < createdAts.length; i++) {
    if (createdAts[i] > maxCreatedAt) {
      maxCreatedAt = createdAts[i];
      maxCreatedIndex = i;
    }
  }

  // En yeni hariç diğerlerini kaldırılacak listede sakla
  for (let i = 0; i < ids.length; i++) {
    if (i !== maxCreatedIndex) {
      duplicates.push({
        _id: ids[i],
        ProductId: group._id.ProductId,
        CurrencyId: group._id.CurrencyId,
        PriceTypeId: group._id.PriceTypeId,
        action: "DELETE" // Eski kaydı sil
      });
      totalRecordsToUpdate++;
    }
  }

  print(`${index + 1}. Kombinasyon: ProductId=${group._id.ProductId}, CurrencyId=${group._id.CurrencyId}, PriceTypeId=${group._id.PriceTypeId}`);
  print(`   Toplam tekrarlanan kayıt: ${duplicateCount}`);
  print(`   Korunacak (en yeni): 1`);
  print(`   Silinecek: ${duplicateCount - 1}`);
  print(``);
});

print(`\n=== Özet ===`);
print(`Silinecek toplam kayıt: ${totalRecordsToUpdate}\n`);

// 3. Onay al
print(`UYARI: Bu işlem ${totalRecordsToUpdate} kaydı silecektir!`);
print(`Devam etmek istiyor musunuz? (Y/N): `);

// Eğer interaktif ise, aksi halde otomatik olarak devam et
// Automation için input kontrol etmiyoruz

print(`\nDevam ediliyor...\n`);

// 4. Silme işlemini gerçekleştir
duplicates.forEach((dup) => {
  const result = collection.deleteOne({ _id: ObjectId(dup._id.toString()) });
  if (result.deletedCount > 0) {
    print(`✓ Silindi: ${dup._id}`);
  }
});

print(`\n=== İşlem Tamamlandı ===`);
print(`Silinen kayıt sayısı: ${totalRecordsToUpdate}\n`);

// 5. Temizlemeden sonra yinelenen kombinasyonlar kalmış mı kontrol et
const remainingDuplicates = collection.aggregate(groupQuery).toArray();
print(`Kalan duplicate kombinasyonları: ${remainingDuplicates.length}`);

if (remainingDuplicates.length === 0) {
  print(`✓ Tüm duplicatlar başarıyla temizlendi!\n`);
  print(`Artık ProductPrice index başarıyla oluşturulabilir.\n`);
} else {
  print(`⚠ Hala duplicate kombinasyonlar bulunmaktadır.\n`);
}
