# Lab 05 — E-commerce App + EF Core Database

**Course:** Full Stack Web Development Lab (BSIT)
**Task:** *Modify your e-commerce application and add a database in it. Use EF Core for
database operations.*

## Deliverable
A clean-architecture e-commerce solution where the application's data is stored in a real
database through **Entity Framework Core**:

- `Domain` — `Category`, `Product`, `Order`, `OrderItem` entities.
- `Application` — use-case services, DTOs and interfaces.
- `Infrastructure` — EF Core `DbContext`, entity configurations, migrations, repositories.
- `Ecommerce.Api` — REST API with full **Product CRUD**, **Categories** and **Orders**.
- `Presentation.Shared` + `Presentation.Web` — shared Blazor UI (a MAUI host is planned).

The database is **SQLite** (`ecommerce.dev.db`), created and seeded automatically on first
run by applying EF Core **migrations**.

## How to run

```bash
cd Lab05-EFCore-Database

# terminal 1 — API (applies migrations + seeds the database)
dotnet run --project src/Ecommerce.Api --no-launch-profile

# terminal 2 — Blazor web preview (shared UI)
dotnet run --project src/Presentation.Web --no-launch-profile
```

| Service | URL |
|---------|-----|
| API root / health | <http://localhost:5088/health> |
| Swagger UI | <http://localhost:5088/swagger> |
| Products / Categories | <http://localhost:5088/api/products> · `/api/categories` |
| Web UI | <http://localhost:5099/products> |

## API endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/products` | List products (with category name) |
| GET | `/api/products/{id}` | Get one product |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |
| GET | `/api/categories` | List categories |
| GET | `/api/orders` | List orders with their items |
| POST | `/api/orders` | Create an order (computes the total) |

## Proof of working (recorded during implementation)

- `dotnet build Ecommerce.slnx` → `Build succeeded. 0 Warning(s), 0 Error(s)`.
- `dotnet ef migrations add InitialCreate` → migration created in `src/Infrastructure/Migrations`.
- `GET /api/products` → 6 seeded products; `GET /api/categories` → 3 categories.
- `POST /api/products` → created id 7; `PUT` → `204`; `DELETE` → `204`; invalid price → `400`;
  missing product → `404`.
- `POST /api/orders` → order total correctly computed as `1299.99 + 2 × 25.50 = 1350.99`.
- **Restart test:** after stopping and restarting the API, the created order was still
  present and the deleted product stayed deleted → the data persists in SQLite.
- Web UI `/products` → `200` and renders the seeded catalog through the API.

## Project structure

```
Lab05-EFCore-Database/
├── Ecommerce.slnx
└── src/
    ├── Domain/                entities (no dependencies)
    ├── Application/           services, DTOs, interfaces
    ├── Infrastructure/        EF Core, migrations, repositories, HTTP client
    ├── Ecommerce.Api/         ASP.NET Core minimal API + Swagger
    ├── Presentation.Shared/   shared Razor components
    └── Presentation.Web/      Blazor web host (Codespaces preview)
```

## Notes / follow-ups
- `Presentation.Client` (.NET MAUI Blazor Hybrid) is planned as the Windows delivery head
  and will reuse `Presentation.Shared` unchanged.
- Labs 1–4 are backfilled into their own folders later; the instructor manuals for Labs 3
  and 4 are the structural reference for this solution.
