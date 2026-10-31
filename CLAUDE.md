# CLAUDE.md — Store.sln Project & Agent Governance Guide

## 1. Project Overview
**Store.sln** is an enterprise-grade B2C & B2B E-Commerce and Periodic Rental platform built on .NET 8.0 (C# 12) with microservices architecture and Clean Architecture (Onion) + DDD patterns.

### Architecture & Services
- **Store.WebUI** (Port: 5079): ASP.NET Core MVC + Razor Views + AdminLTE. Connects to microservices via Refit + DelegatingHandler (Bearer Token).
- **Store.Catalog** (Port: 7301): MongoDB 7.x document database. Products, categories, images, brands.
- **Store.Discount** (Port: 7302): PostgreSQL + Dapper. Coupon and discount management.
- **Store.Location** (Port: 7303): PostgreSQL + EF Core (Clean Architecture). Countries, cities, districts, warehouses.
- **Store.Order** (Port: 7304): PostgreSQL + EF Core + MediatR/CQRS. Order management and state machine.
- **Store.B2B** (Port: 5173): PostgreSQL + EF Core. Dealer financial management, credit limits, discounts.
- **Store.Basket** (Port: 7306): Redis caching. Cart management with 30-day TTL.
- **Store.Favorite** (Port: 7307): PostgreSQL + EF Core + CQRS. User favorites with token-based IDOR protection.
- **Store.Rental** (Port: 7308): PostgreSQL + EF Core + CQRS. Periodic rental management, date conflict checking, calendar engine.
- **Store.IdentityServer** (Port: 5001): PostgreSQL + IdentityServer4 (OAuth 2.0 / OpenID Connect, JWT Bearer).
- **Shared Libraries**: `Store.Shared` (BaseEntity, Auditing, Soft-Delete, AesEncryptionHelper), `Store.DtoLayer`.

---

## 2. Agent Governance System (.agents/)
This project operates under a strict **1 Orchestrator + 5 Specialists** governance model.

### Specialist Roles:
1. **Agent_Gravity**: Senior Project Manager & Orchestrator. Coordinates tasks, manages handoffs, enforces decision gates.
2. **Architect Agent**: Clean Architecture boundaries, CQRS handlers, API contracts, dependencies.
3. **Business Analyst Agent**: Pricing models (Sale, Rental, Wholesale), rental conflict logic, order workflows.
4. **DBA Agent**: PostgreSQL & MongoDB schemas, EF Core migrations, indexing, soft-delete filters, performance.
5. **Security Agent**: JWT/OAuth2 policies, IDOR prevention, KVKK compliance (AES-256), secret management.
6. **Test Engineer Agent**: xUnit unit & integration testing, validation verification, test evidence.

### Hard Constraints:
- **No Plaintext Secrets**: No passwords or connection strings in config files or Git.
- **Strict Bounded Contexts**: No direct `ProjectReference` between microservice Domain layers.
- **KVKK Compliance**: Sensitive data (such as `IdentityNumber`) MUST be encrypted using AES-256.
- **Evidence & Verification**: All code changes require build and test verification before completion.
- **Truth Hierarchy**: Working code/schema > Approved ADRs > Architecture maps > Business rules > Project status docs.

---

## 3. Analysis Directive for Claude
When analyzing this project, produce a detailed assessment addressing:
1. **Strong Points (Güçlü Yanlar)**: Architectural patterns, domain isolation, modern tech stack choices, security hardening completed to date.
2. **Weak Points & Technical Debts (Zayıf Yanlar ve Riskler)**: Missing API Gateway (YARP), lack of asynchronous messaging (RabbitMQ/MassTransit), testing coverage gaps, IdentityServer4 deprecation.
3. **Missing Modules (Eksikler)**: Payment integration (iyzico/PayTR), centralized inventory/stock engine, shipment/logistics integration, B2B admin UI.
4. **Actionable Roadmap (Önceliklendirilmiş Yol Planı)**: Step-by-step phased execution (Phase 1: Hardening, Phase 2: Core Business Workflows, Phase 3: Infrastructure & DevOps, Phase 4: Scale).

