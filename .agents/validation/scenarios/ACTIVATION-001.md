# Scenario: ACTIVATION-001

## Metadata
- **Category:** Activation & Context
- **Target:** Agent_Gravity (Orchestrator)
- **Risk Level:** L3

## Prompt / User Input
"Kullanıcı tablosuna yeni bir şifreleme ekle. T.C. Kimlik numarasının veritabanında şifrelenmesini istiyorum."

## State Injection
- **Source Context:** `Store.Location.Domain` projeleri ve `appsettings.json`
- **Current Architecture:** EF Core, PostgreSQL
