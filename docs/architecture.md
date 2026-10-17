# Architecture

The application follows **clean architecture**: dependencies point inward, and the core
business rules do not know about databases or UI.

```
┌──────────────────────────────────────────────────────────────┐
│  Presentation (Blazor)            Infrastructure              │
│  ├─ Presentation.Shared  ─┐       ├─ EF Core DbContext        │
│  ├─ Presentation.Web      │       ├─ Repositories              │
│  └─ Presentation.Client   │       ├─ Migrations                │
│     (MAUI, Windows)       │       └─ HttpProductApiClient      │
│                           │                │                   │
│  Ecommerce.Api (Minimal API) ──────────────┘                   │
│                           │                │                   │
│              ┌────────────▼────────────────▼───────┐           │
│              │            Application              │           │
│              │  DTOs · interfaces · services        │           │
│              └────────────────┬────────────────────┘           │
│                               │                                │
│              ┌────────────────▼────────────────────┐           │
│              │              Domain                 │           │
│              │  Category · Product · Order · …      │           │
│              └─────────────────────────────────────┘           │
└──────────────────────────────────────────────────────────────┘
```

## Layers

| Layer | Responsibility | Depends on |
|-------|----------------|-----------|
| **Domain** | Entities and invariants. Plain C#, zero dependencies. | — |
| **Application** | Use cases (`ProductService`, …), DTOs, and the interfaces the outside world must implement (`IProductRepository`, `IProductApiClient`). | Domain |
| **Infrastructure** | EF Core `DbContext`, entity configurations, migrations, repository implementations, and the HTTP client used by UI hosts. | Application, Domain |
| **Ecommerce.Api** | ASP.NET Core Minimal API — maps HTTP to Application services. | Application, Infrastructure |
| **Presentation.Shared** | Razor components (pages). Talks only to `IProductApiClient`, never to EF Core. | Application |
| **Presentation.Web** | Blazor Web host for running/previewing the shared UI (works on Linux/Codespaces). | Presentation.Shared, Infrastructure |
| **Presentation.Client** | .NET MAUI Blazor Hybrid host (Windows). Reuses the same shared UI. | Presentation.Shared, Infrastructure |

## Key ideas to be able to defend
- **The Domain has no references.** That is what makes it testable and framework-independent.
- **Services depend on interfaces, not EF Core.** `ProductService` never mentions `DbContext`.
- **The UI never talks to the database.** It goes through `IProductApiClient` → the API →
  Application → Infrastructure. Both the Blazor and the MAUI host use the same client.
- **DTOs cross boundaries.** Entities stay inside; the API and UI exchange DTOs.

See `docs/adr/` for the reasoning behind each structural decision.
