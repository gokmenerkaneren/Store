# Agent Metrics (Ajan Performans Metrikleri)

## Amaç
Oluşturulan "Agent Governance" sisteminin gerçekten işe yarayıp yaramadığını, ajanların ne kadar verimli çalıştığını ve süreçte nerede tıkanıklık (bottleneck) yaşandığını ölçmektir. Bu metrikler `WEEKLY_ENGINEERING_GOVERNANCE.md` toplantılarının (audit) ayrılmaz bir parçasıdır.

## İzlenecek Metrikler

### 1. Aktivasyon Doğruluğu (Activation Accuracy)
- **Yanlış Aktivasyon (Wrong Activation):** Görevle alakasız bir ajanın çağrılması (Örn: Sadece CSS değişikliği için DBA çağrılması).
- **Eksik Aktivasyon (Missed Activation):** Kritik bir güvenlik değişikliğinde Security ajanı çağrılmadan kodun geçmesi.
- *Hedef:* Yanlış/Eksik aktivasyon oranının %0 olması.

### 2. İletişim & Devir (Handoff) Sağlığı
- **Tekrarlanan Devir (Repeated Handoff):** Ajanların aynı görevi "Sen yap, hayır sen yap" şeklinde birbirine atma sayısı.
- **Başarısız Devir (Failed Handoff):** Handoff dokümanı oluşturulmadan veya kanıt (Evidence) eksikliğiyle topun diğer ajana atılması.
- *Hedef:* Tek görevde ortalama Handoff sayısının 2'yi geçmemesi.

### 3. Karar ve İnsan Müdahalesi (Decision & Human Gate)
- **Kullanıcıya Danışma Sıklığı (Decision Gate Frequency):** Ajanların kendi kendine çözemeyip Karar Kapısı (Decision Package) ile Kullanıcıya (Gökmen'e) başvurma sayısı. 
  - *Not:* Fazla olması ajanların yeterince otonom olmadığını, az olması ise riskleri göz ardı ettiklerini gösterebilir.
- **Onay Bekleme Süresi:** Bir Karar Kapısı açıldıktan sonra sistemin bekleme süresi (Agent State: `WAITING_FOR_DECISION`).

### 4. Hata ve Yeniden İşleme (Failure & Rework)
- **Yeniden İşleme Oranı (Rework Rate):** Test başarısızlığı (Test Defect) veya Kod Hatası (Code Defect) nedeniyle Architect veya Test Engineer'ın kodu ikinci kez yazmak zorunda kalması.
- **Halüsinasyon / Belgesel Sapma (Documentation Drift):** Ajanın projede var olmayan bir teknoloji paketini (örn. EntityFramework 6) hayal edip kod yazmaya çalışması. (Single Source of Truth ihlali).

## Değerlendirme
Bu metrikler haftalık toplantılarda değerlendirilecek. Hata veya limit aşımları yüksekse, ilgili ajanın `SKILL.md` kuralları veya `CONTEXT_LOADING_PROTOCOL.md` güncellenecektir (Governance Evolution).
