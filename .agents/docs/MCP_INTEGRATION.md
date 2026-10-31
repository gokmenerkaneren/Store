# MCP (MODEL CONTEXT PROTOCOL) SUNUCULARI DOKÜMANTASYONU
# ==========================================================
# Hazırlayan: Agent Gravity
# Tarih: 27 Ekim 2026
# Amaç: Store.sln mikroservis ekosistemi ve veritabanı yönetimi için kurulan MCP sunucularının listesi, konfigürasyonu ve kullanım kılavuzu.

---

## 1. AKTİF MCP SUNUCULARI VE GÖREVLERİ

Projenin mimarisi (PostgreSQL, MongoDB, Redis, Mikroservis API'leri ve Sürüm Kontrolü) doğrultusunda aşağıdaki 5 adet MCP sunucusu sisteme entegre edilmiştir:

| Sunucu Adı | Sağlayıcı / Paket | Bağlantı / Hedef | Kullanım Amacı |
| :--- | :--- | :--- | :--- |
| **`postgres`** | `@modelcontextprotocol/server-postgres` | `postgresql://postgres:sa435558@localhost:5432/StoreIdentityDB` | PostgreSQL şemalarını, tablolarını inceleme, SQL çalıştırma ve veri kontrolü. |
| **`mongodb`** | `mongodb-mcp-server` | `mongodb://localhost:27017/StoreMD` | MongoDB `StoreMD` koleksiyonlarını (Products, Categories, Prices vb.) ve BSON dokümanlarını doğrudan sorgulama. |
| **`redis`** | `@modelcontextprotocol/server-redis` | `redis://localhost:6379` | Redis sepet verilerini (Keys, TTL, Get/Set) yönetme ve anlık önbellek takibi. |
| **`github`** | `@modelcontextprotocol/server-github` | GitHub API | GitHub repository yönetimi, issue/PR oluşturma, kod geçmişi inceleme. *(PAT token opsiyonel)* |
| **`postman`** | `@postman/postman-mcp-server` | Postman API | Mikroservis endpoint testlerini ve Postman koleksiyonlarını çalıştırma/doğrulama. *(API Key opsiyonel)* |

---

## 2. KONFİGÜRASYON DOSYA YOLLARI

Tüm sunucular aşağıdaki üç kritik konuma kaydedilerek tam erişilebilirlik sağlanmıştır:

1. **Global Antigravity Konfigürasyonu:**  
   `C:\Users\devel\.gemini\config\mcp_config.json`
2. **Proje İçi Yönetişim (.agents):**  
   `C:\User\Projects\Store\.agents\mcp_config.json`
3. **Workspace Kökü (IDE Desteği):**  
   `C:\User\Projects\Store\.mcp.json`

---

## 3. İSTEĞE BAĞLI TOKEN VE API KEY TANIMLARI

Veritabanı sunucuları (`postgres`, `mongodb`, `redis`) yerel kimlik bilgileriyle tam entegre çalışmaktadır.  
Dış bulut servisleri için:
- **GitHub:** `mcp_config.json` içindeki `GITHUB_PERSONAL_ACCESS_TOKEN` alanına kendi GitHub token'ınızı yazabilirsiniz.
- **Postman:** `mcp_config.json` içindeki `POSTMAN_API_KEY` alanına kendi Postman Workspace API Key'inizi yazabilirsiniz.

