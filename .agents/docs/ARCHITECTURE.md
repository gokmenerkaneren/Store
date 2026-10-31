# ARCHITECTURE — STORE.SLN TEKNİK MİMARİ HARİTASI
# ═══════════════════════════════════════════════════════
# Mikroservis yapısı, katman mimarisi, servis portları ve iletişim protokollerini tanımlar.
#
# Son Güncelleme: 23 Eylül 2026

---

## 1. GENEL MİMARİ GÖRÜNÜM

```
                              ┌─────────────────────┐
                              │   Store.WebUI        │
                              │   (ASP.NET MVC)      │
                              │   Port: 5079         │
                              └────────┬────────────┘
                                       │ Refit + HttpClientFactory
                    ┌──────────────────┼──────────────────────┐
                    │                  │                      │
              ┌─────▼──────┐    ┌──────▼──────┐    ┌─────────▼─────────┐
              │ Catalog     │    │ Location    │    │ IdentityServer    │
              │ Port: 7301  │    │ Port: 7303  │    │ Port: 5001        │
              │ MongoDB     │    │ PostgreSQL  │    │ PostgreSQL        │
              └─────────────┘    └─────────────┘    └───────────────────┘
                    │                  │
         ┌─────────┼──────────┬───────┼──────────┐
         │         │          │       │          │
   ┌─────▼──┐  ┌──▼───┐   ┌───▼──┐ ┌──▼───────┐ ┌▼────────┐ ┌──────────┐
   │Discount│  │Order │   │Rental│ │B2B       │ │Favorite │ │Basket    │
   │Pt:7302 │  │Pt:7304│  │Pt:5010│ │Pt:5173   │ │Pt:5227  │ │Pt:7306   │
   │PgSQL   │  │PgSQL  │  │PgSQL │ │PgSQL     │ │PgSQL    │ │Redis     │
   └────────┘  └───────┘  └──────┘ └──────────┘ └─────────┘ └──────────┘
```

---

## 2. SERVİS KAYIT DEFTERİ

| # | Servis | Port | Veritabanı | DBMS | Mimari | Durum |
|:--|:---|:---|:---|:---|:---|:---|
| 0 | **Store.Gateway** | 7000 | — | — | YARP Reverse Proxy | ✅ Aktif |
| 1 | **Store.WebUI** | 5079 | — | — | MVC Frontend | ✅ Aktif |
| 2 | **Store.Catalog** | `7301` | `StoreMD` | MongoDB 7.x | Monolitik | ✅ Aktif |
| 3 | **Store.Discount** | `7302` | `StoreDiscountDb` | PostgreSQL 16.x | Monolitik (Dapper+EF) | ✅ Aktif |
| 4 | **Store.Location** | `7303` | `StoreLocationDb` | PostgreSQL 16.x | Clean Architecture | ✅ Aktif |
| 5 | **Store.Order** | `7304` | `StoreOrderDb` | PostgreSQL 16.x | Clean Architecture | ✅ Aktif |
| 6 | **Store.Basket** | `7306` | — | Redis | Monolitik | ✅ Aktif |
| 7 | **Store.IdentityServer** | `5001` | `StoreIdentityDB` | PostgreSQL 16.x | IdentityServer4 | ✅ Aktif |
| 8 | **Store.B2B** | 7305 | StoreB2BDb | PostgreSQL 16.x | Clean Architecture | ✅ Aktif |
| 9 | **Store.Favorite** | `7307` | `StoreFavoriteDb` | PostgreSQL 16.x | Clean Architecture | ✅ Aktif |
| 10 | **Store.Rental** | `7308` | `StoreRentalDb` | PostgreSQL 16.x | Clean Architecture | ✅ Aktif |
| 11 | **Store.Payment** | 7309 | — | In-Memory / Mock | Microservice | ✅ Aktif |

> **Mimari Sınır Notu (ADR-004):** Sistem endüstriyel MRP/üretim planlama kapsamını içermez. Stok yönetimi depo bazlı ürün giriş/çıkış ve kiralama rezervasyonu sınırlarıyla yönetilir.

---

## 3. SERVİS KATMAN YAPILARI

### 3.1 Clean Architecture Servisleri (Location, Order, B2B, Favorite, Rental)

```
Store.<Servis>.Domain        → BaseEntity, Entities, Enums, Value Objects (Bağımsız Çekirdek)
Store.<Servis>.Application   → CQRS Commands/Queries, MediatR Handlers, DTOs, FluentValidation
Store.<Servis>.Persistence   → DbContext, EF Core Configurations, Migrations, Repositories
Store.<Servis>.WebApi        → Controllers, Program.cs, appsettings.json
```

**Kurallar:**
- Controller → MediatR → Handler → Repository zinciri izlenir
- Controller'dan doğrudan DbContext veya Repository çağrılmaz
- Her Command için FluentValidation Validator yazılır

### 3.2 Monolitik Servisler (Catalog, Discount)

```
Store.<Servis>/
  ├── Controllers/       → API Controller'ları
  ├── Dtos/              → Data Transfer Objects
  ├── Entities/          → MongoDB/EF Entity sınıfları
  ├── Mapping/           → AutoMapper profilleri
  ├── Services/          → İş mantığı servisleri (Interface + Implementation)
  ├── Settings/          → Konfigürasyon sınıfları
  └── Program.cs         → Uygulama başlangıç noktası
```

**Kurallar:**
- Controller → Service (Interface) → Repository/Collection zinciri izlenir
- AutoMapper ile Entity ↔ DTO dönüşümü yapılır

---

## 4. SERVİSLER ARASI İLETİŞİM

### 4.1 Mevcut Durum
- **Protokol:** HTTP/REST (senkron)
- **İstemci:** Refit + HttpClientFactory
- **Yön:** WebUI → Mikroservisler (tek yönlü)
- **Gateway:** YOK (WebUI her servise doğrudan bağlanıyor)

### 4.2 Planlanan İyileştirmeler
- **API Gateway:** YARP veya Ocelot ile merkezi yönlendirme
- **Asenkron Mesajlaşma:** RabbitMQ / MassTransit (sipariş → stok düşme olayı vb.)
- **Önbellekleme:** Redis cache (sık erişilen katalog verileri)

---

## 5. PAYLAŞILAN KÜTÜPHANELER

| Kütüphane | Konum | Amaç |
|:---|:---|:---|
| **Store.Shared** | `Libraries/Store.Shared/` | Ortak BaseEntity, Enums, Extension Methods, Helper sınıflar |
| **Store.DtoLayer** | `Store.DtoLayer/` | WebUI'ın kullandığı tüm DTO sınıfları (Katalog, Sipariş, İndirim vb.) |

**Bounded Context Kuralı:** Bir mikroservis, başka bir servisin Domain katmanına doğrudan `ProjectReference` veremez. Paylaşılacak tipler `Store.Shared` veya `Store.DtoLayer` üzerinden aktarılır.

---

## 6. WEBUI YAPISI

```
Store.WebUI/
  ├── Areas/Admin/                → Admin Paneli
  │   ├── Controllers/            → Admin controller'ları
  │   ├── Views/                  → Admin Razor view'ları
  │   └── ViewComponents/         → Admin panel bileşenleri
  ├── Controllers/                → Kullanıcı tarafı (Default, Product, Collection, UILayout)
  ├── Helpers/                    → ErrorHelper, NavigationHelper, PermissionHelper
  ├── Services/                   → Refit interface'leri ve implementasyonları
  ├── ViewComponents/             → UILayout ve frontend bileşenleri
  └── Views/                      → Kullanıcı tarafı Razor view'ları
```

### 6.1 Admin Panel ViewComponent Yapısı
```
_AdminLayoutComponentLeftMenuPartial
_AdminLayoutComponentHeaderPartial
_AdminLayoutComponentSideBarPartial
_AdminLayoutComponentCardPartial
_AdminLayoutComponentHeadPartial
_AdminLayoutComponentScriptPartial
_AdminLayoutComponentPreLoaderPartial
```

---

## 7. DOCKER ALTYAPISI

```yaml
# docker-compose.yml (Mevcut)
services:
  mongodb:     → Port: 27017
  postgresql:  → Port: 5432
  redis:       → Port: 6379
```

> **Not:** Servisler henüz Docker container'ları içinde çalışmıyor. Şu an geliştirme ortamında doğrudan `dotnet run` ile çalışıyorlar. Docker compose yalnızca veritabanı motorları için kullanılıyor.

---

> **Güncelleme Kuralı:** Yeni bir servis eklendiğinde, port numarası değiştiğinde veya mimari karar alındığında bu doküman güncellenir.

---

## 4. ASENKRON İLETİŞİM VE OUTBOX MİMARİSİ

```
   ┌────────────────────────────────────────────────────────┐
   │                    Store.Order (PostgreSQL)            │
   │  ┌──────────────────┐         ┌─────────────────────┐  │
   │  │ Order & Details  │  Atomik │ MassTransit Outbox  │  │
   │  │ DbContext Save   │◄───────►│ (Inbox/OutboxState) │  │
   │  └──────────────────┘         └──────────┬──────────┘  │
   └──────────────────────────────────────────┼─────────────┘
                                              │ Background Bus Outbox
                                              ▼
                                 ┌─────────────────────────┐
                                 │   RabbitMQ (MessageBus) │
                                 │   Port: 5672 / 15672    │
                                 └────────────┬────────────┘
                                              │ Event Delivery
                                              ▼
   ┌────────────────────────────────────────────────────────┐
   │                    Store.Catalog (MongoDB)             │
   │  ┌──────────────────────────────────────────────────┐  │
   │  │ OrderCreatedConsumer                             │  │
   │  │ -> Variant Stock Check (Optimistic Concurrency)  │  │
   │  │ -> $inc Rollback on Failure                      │  │
   │  │ -> Publish StockReserved / Failed Event          │  │
   │  └──────────────────────────────────────────────────┘  │
   └────────────────────────────────────────────────────────┘
```

- **Broker:** RabbitMQ 3.x (Management Dashboard: 15672).
- **Transport:** MassTransit 8.2.5.
- **Kritik Desen (Outbox):** `Store.Order` mikroservisinde EntityFramework Core Outbox aktiftir. Sipariş ile mesaj aynı PostgreSQL transaction'ında commit edilir (Dual-Write problemi çözüldü).
- **Koreografi Tabanlı Kompansasyon:** Merkezi Saga yerine hafif ve ölçeklenebilir koreografi modeli uygulanır:
  1. `OrderCreatedIntegrationEvent` (Order -> Catalog)
  2. `StockReservedIntegrationEvent` (Catalog -> Order: Durum `Processing` olur)
  3. `StockReservationFailedIntegrationEvent` (Catalog -> Order: Durum `Cancelled` olur ve iptal sebebi notlara işlenir)

