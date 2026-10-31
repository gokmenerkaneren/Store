# Error Recovery & Failure Classification (Hata Yönetimi ve Sınıflandırma)

## Amaç
Bir hata meydana geldiğinde ajanların "Hata oldu, tekrar deniyorum" döngüsüne girmesini engellemek; bunun yerine hatanın türünü (Classification) belirleyip doğru çözüm yolunu seçmelerini sağlamaktır.

## Hata Sınıflandırması (Failure Classification)

1. **CODE_DEFECT (Kod Hatası):**
   - *Durum:* Derleme (Build) hatası, sözdizimi hatası, eksik interface.
   - *Aksiyon:* Hata, kodu yazan ajana (Architect/BA) geri gönderilir.

2. **TEST_DEFECT (Test/Mantık Hatası):**
   - *Durum:* Kod derleniyor ancak Unit/Integration testi başarısız.
   - *Aksiyon:* Business Analyst'e veya Test Engineer'a (test hatalıysa) gönderilir.

3. **ENVIRONMENT_FAILURE (Çevre/Araç Hatası):**
   - *Durum:* Local veritabanına bağlanılamıyor, araç (tool) çalışmıyor.
   - *Aksiyon:* Retry (Tekrar) yapılmaz, doğrudan Kullanıcıya (BLOCKED) raporlanır.

4. **REQUIREMENT_DEFECT (Gereksinim / İstek Çelişkisi):**
   - *Durum:* Kullanıcının (Gökmen'in) talebi belirsiz, çelişkili veya eksik (Örn: "Herkese açık olsun ama sadece admin girsin").
   - *Aksiyon:* Kod hatası aranmaz. Agent Gravity, problemi `DECISION_SCHEMA.md` üzerinden açıklayarak Kullanıcıdan (Gökmen) açıklama talep eder.

5. **DATA_DEFECT (Veri Kaynaklı Hata):**
   - *Durum:* Kod, test ve migration DOĞRU çalışıyor, ancak test/mock verisinde veya Production'daki veride bir uyumsuzluk var.
   - *Aksiyon:* DBA ve Test Engineer inceler, gerekirse veri temizleme senaryosu önerilir.

6. **EXTERNAL_DEPENDENCY_FAILURE (Dış Bağımlılık Hatası):**
   - *Durum:* Harici bir ödeme servisi, SMTP mail servisi veya IdentityServer'ın ayakta olmaması nedeniyle testlerin/kodun çökmesi.
   - *Aksiyon:* Hata kodda aranmaz, çevre hatası olarak Kullanıcıya iletilir.

7. **GOVERNANCE_ERROR (Yönetişim İhlali):**
   - *Durum:* Değişiklik, Architecture veya Business Rules dokümanlarını açıkça deliyor.
   - *Aksiyon:* Görev iptal edilir. Ajan kurala uymaya zorlanır.

## Tekrarlama Sınırı (Loop Protection)
Aynı hata (Örn: CODE_DEFECT) art arda `MAX_AGENT_RETRY = 2` kez düzeltilemiyorsa görev `BLOCKED` durumuna düşer.
