# Store.sln — Agent Governance v1.0

**Purpose:** Store.sln için kontrollü, izlenebilir ve sürdürülebilir AI destekli yazılım mühendisliği çalışma modeli.

## Tasarım ilkesi

Bu paket, mevcut Antigravity tasarım dokümanındaki "1 Orkestra Şefi + 5 Uzman" yaklaşımını bir **Governance + Orchestration** sistemine dönüştürür. Kaynak dokümanda tespit edilen eksikler; aktivasyon kuralları, standart çıktılar, periyodik review, ajanlar arası iletişim, görev parçalama/paralel çalışma ve hata kurtarma protokolleri olarak ele alınmıştır.

## Uzmanlar

- Agent_Gravity — Orchestrator / Coordinator
- Architect Agent
- Business Analyst Agent
- DBA Agent
- Security Agent
- Test Engineer Agent

## Kritik ilke

Ajanlar proje hakkında kaynaklarda bulunmayan bir gerçeği varsayamaz. Çelişki veya belirsizlik varsa `CONFLICT`, `OPEN_DECISION` veya `APPROVAL_REQUIRED` durumu oluşturulur.

## Kurulum

1. Bu paketin `.agents/` klasörünü Store.sln köküne alın.
2. Mevcut `.agents/docs/` dosyalarınızı **silmeden** `docs/` altına entegre edin.
3. `docs/PROJECT_INTEGRATION_GUIDE.md` ve `docs/MIGRATION_CHECKLIST.md` adımlarını uygulayın.
4. Önce Governance dry-run yapın; doğrudan kod üretimine geçmeyin.
5. Kaynak dokümanda belirtilen tutarsızlıkları doğrulamadan otomatik düzeltmeyin.

## v1.0 kapsamı

Bu paket bir "kendi kendini değiştiren ajan" sistemi değildir. Ajan sistemi değişiklikleri proposal → impact analysis → approval → versioning sürecinden geçer.
