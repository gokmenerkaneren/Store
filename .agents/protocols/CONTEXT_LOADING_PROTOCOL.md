# Context Loading Protocol (Bağlam Yükleme Stratejisi)

## Amaç
Ajanların her görevde on binlerce satırlık Markdown okumasını engellemek, ancak sadece dokümanlara (Markdown) bağımlı kalmalarını engelleyerek gerçek kaynaklara (Kod, DB, Log) erişimlerini de düzenlemektir.

## Context Profilleri (Context Architecture)
Bir görev verildiğinde Agent Gravity, ilgili ajana aşağıdaki bağlamların hangilerine erişmesi gerektiğini profiller.

### 1. Governance Context (Kurallar ve Protokoller)
**İçerik:** `GLOBAL_RULES.md`, `Agent_Gravity.md`, `TRUTH_HIERARCHY.md`, `TASK_LIFECYCLE.md`
**Durum:** Her ajana yüklenmesine gerek yoktur. Sadece "Orchestrator Core" veya "Governance Core" kararlarında, ağırlıklı olarak Agent_Gravity kullanır. Uzman ajanlar kendi `SKILL.md` kurallarını okur.

### 2. Documentation Context (Proje Dokümantasyonu)
**İçerik:** `TECH_STACK.md`, `ARCHITECTURE.md`, `BUSINESS_RULES.md`, `DATABASE_MAP.md`
**Durum:** Ajanın tipine göre yüklenir (Örn: DBA'e `DATABASE_MAP`, BA'e `BUSINESS_RULES`).

### 3. Project Source Context (Gerçek Proje Kaynağı) - *ZORUNLU ERİŞİM*
**İçerik:** `*.cs` (C# Kaynak Kodları), `appsettings.json`, `*.csproj`, `Git geçmişi`.
**Durum:** Bir ajan "Mimarimiz şöyledir" diye karar vermeden önce MUTLAKA gerçek kaynak kodu incelemekle yükümlüdür. Markdown dokümanlar sadece rehberdir. (Bkz: `TRUTH_HIERARCHY.md`).

### 4. Runtime & Evidence Context (Kanıt ve Loglar)
**İçerik:** `dotnet build` logları, `dotnet test` sonuçları, CLI çıktıları.
**Durum:** Ajan `VERIFYING` durumundayken zorunlu olarak bu bağlama erişir.

## Uygulama Kuralı
"Ajanlar yalnızca Markdown dokümanlarına erişebilir" kuralı YANLIŞTIR. Dokümanlar rehberdir, **Source Code ve Runtime Loglar** her zaman gerçek referanstır. Ajanlar Context Profiline uygun olarak Source Context okuması yapmak zorundadır.
