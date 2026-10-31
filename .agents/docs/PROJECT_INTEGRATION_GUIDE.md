# PROJECT INTEGRATION GUIDE

Bu paket mevcut Store.sln `.agents` yapısının yerine körlemesine geçirilmemelidir.

## 1. Korunacak proje gerçeklikleri

Kaynak dokümanda mevcut olduğu belirtilen temel dosyalar:
- ARCHITECTURE.md
- BUSINESS_RULES.md
- DATABASE_MAP.md
- PROJECT_CONTEXT.md
- PROJECT_STATUS.md

Bunların mevcut sürümleri korunmalı ve Governance tarafından referans alınmalıdır.

## 2. Governance dosyalarının rolü

`rules/` = davranış kuralları
`skills/` = uzmanlık kuralları
`protocols/` = süreç kuralları
`docs/` = proje gerçekliği / entegrasyon açıklamaları
`templates/` = standart çıktı biçimleri
`audits/` = review kayıtları

## 3. İlk entegrasyon

Önce dry-run: hiçbir proje kodu değiştirilmeden görev sınıflandırma, context discovery, conflict detection ve agent activation test edilir.
