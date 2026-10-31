# Loop Protection (Kısır Döngü Koruması)

## Amaç
Otonom ajanların kendi aralarında anlaşamadığı veya aynı hatayı sürekli çözmeye çalışıp başaramadığı durumlarda sistemin sonsuz döngüye (infinite loop) girmesini engellemektir.

## Limitler (Thresholds)
Ajanlar arası iletişim ve tekrar deneme (retry) süreçleri aşağıdaki katı kurallara tabidir:

- `MAX_HANDOFF_COUNT = 3`: İki ajan aynı konu üzerinde birbirine en fazla 3 kez görev (handoff) atabilir.
- `MAX_AGENT_RETRY = 2`: Bir ajan (örneğin Test Engineer veya Architect) aynı hatayı (örn: derleme hatası) çözmek için arka arkaya en fazla 2 kez kod değişikliği deneyebilir.
- `MAX_CONFLICT_ROUNDS = 2`: İki veya daha fazla ajanın fikir ayrılığı yaşayıp uzlaşamadığı müzakere (conflict) döngüsü 2'yi geçemez.

## Sınır Aşımı (Threshold Exceeded) Durumunda Ne Olur?
Eğer yukarıdaki limitlerden herhangi biri aşılırsa:
1. **Durdur (Halt):** Agent_Gravity (Orkestratör) anında işlemi durdurur. Ajanların kendi başlarına sorunu çözmeye devam etmelerine izin verilmez.
2. **Kullanıcı Karar Kapısı (USER_APPROVAL_GATE):** Yaşanan tıkanıklık derlenir. Hangi ajanın ne önerdiği, hatanın veya çatışmanın ne olduğu doğrudan Kullanıcıya (Gökmen'e) özet olarak sunulur.
3. **Bekleme Durumu (Blocked State):** Kullanıcı (Gökmen) bir yön gösterene veya kararı onaylayana kadar sistem ilgili görev için bekleme durumuna geçer.
