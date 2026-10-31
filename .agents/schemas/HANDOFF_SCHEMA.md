# Handoff Schema (Devir Veri Sözleşmesi)

## Format
Bir ajan görevini tamamlayıp veya bir parçasını yapıp topu diğerine attığında bu sözleşme kullanılır.

```markdown
**Handoff ID:** HND-YYYYMMDD-XXXX (Örn: HND-20261003-001)
**Task ID:** [Bağlı olduğu Task ID]
**Parent Handoff ID:** [Eğer bu bir geri dönüş ise önceki HND ID]
**Source Agent:** [AGT-XXX]
**Target Agent:** [AGT-XXX]
**Timestamp:** [ISO 8601]
**Status:** [PENDING / ACCEPTED / REJECTED]
**Payload:**
  - Neler yapıldı?
  - Hangi dosyalar değişti?
  - Hedef ajandan ne bekleniyor?
**Evidence ID:** [Varsa sağlanan kanıtın ID'si: EVD-XXXX]
```

## Kullanım Kuralı
Ajanlar arası iletişim sadece bu kimliklendirilmiş sözleşme üzerinden yapılır. `Source Agent` ve `Target Agent` alanlarında Agent Registry'deki canonical ID'ler (Örn: AGT-DBA) kullanılmalıdır.
