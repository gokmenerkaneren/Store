# AGENT ACTIVATION MATRIX

| Görev | Lider | Destek |
|---|---|---|
| Yeni servis/modül | Architect | DBA, Security, Test |
| Endpoint/controller | Architect | Security, Test |
| Entity/migration | DBA | Architect, Test |
| İş akışı | Business Analyst | Architect, Test |
| Validasyon | Business Analyst | Security, Test |
| Code review/refactoring | Architect | Test, DBA, Security |
| Security audit | Security | Architect, Test |
| Test yazma | Test Engineer | İlgili uzman |
| Performans | DBA | Architect, Test |
| Config/secret | Security | Architect |
| Bugfix | Architect | İlgili uzman + Test |
| Dokümantasyon | Gravity | İlgili uzman |

## Kural

Bir görev birden fazla alanı etkiliyorsa yalnızca lider uzman çalışmaz; etkilenen uzmanlar validation rolünde aktive edilir.
