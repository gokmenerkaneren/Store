# TECH STACK — STORE.SLN TEKNOLOJİ YIĞINI
# ══════════════════════════════════════════════════
# Projede kullanılan kütüphaneler, versiyonları ve altyapı bileşenleri.

---

## 1. TEMEL TEKNOLOJİLER
- **Framework:** .NET 8.0 (C# 12)
- **WebUI:** ASP.NET Core MVC
- **ORM:** Entity Framework Core 8.0 & Dapper
- **Veritabanları:** PostgreSQL 16.x, MongoDB 7.x, Redis

## 2. TEMEL NUGET PAKETLERİ
- `MediatR` - CQRS ve Event yönetimi için
- `FluentValidation.AspNetCore` - Doğrulama işlemleri için
- `AutoMapper` - Entity ve DTO dönüşümleri için
- `Refit` - Typed HttpClient üzerinden servisler arası iletişim için
- `IdentityServer4` - Kimlik doğrulama ve yetkilendirme (Oturum yönetimi)
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL EF provider
- `MongoDB.Driver` - MongoDB bağlantıları için

## 3. ARAÇLAR VE ORTAMLAR
- Docker (Geliştirme ortamı veritabanları)
- PGAdmin 4 / MongoDB Compass / Redis Insight
- Swagger (Mikroservis API testleri)

> **Güncelleme Kuralı:** Yeni bir paket/altyapı eklendiğinde buraya işlenmelidir.
