# Implementation Log — Lab 05

Living document. Newest entries at the bottom.

## 2026-10-17 — Plan approved
Agreed with the maintainer: e-commerce domain, **server-side EF Core + SQLite**, minimal
scaffold now with Labs 1–4 backfilled later, and a shared-UI/two-host layout so the UI can
be previewed in Codespaces (see `../../docs/adr/0003-shared-ui-two-hosts.md`).

## 2026-10-17 — Toolchain check
- `.NET SDK 10.0.401` present.
- `dotnet-ef` was **not** installed → `dotnet tool install --global dotnet-ef` (v10.0.13).

## 2026-10-17 — Part A: Domain → Application → Infrastructure → API
- Scaffolded `Domain`, `Application`, `Infrastructure`, `Ecommerce.Api` and added them to
  `Ecommerce.slnx`.
- Added packages: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`
  (Infrastructure) and `Swashbuckle.AspNetCore` (Api).
- Domain entities: `Category`, `Product`, `Order`, `OrderItem`.
- Application: DTOs, repository/`IProductApiClient` interfaces, `ProductService`,
  `CategoryService`, `OrderService`, hand-written mapping extensions.
- Infrastructure: `EcommerceDbContext`, four `IEntityTypeConfiguration`s with **seed data**
  via `HasData`, three repositories, `AddInfrastructure(connectionString)`.
- API: endpoint groups for products/categories/orders; applies migrations at startup;
  Swagger UI enabled in development.

**Decisions**
- Seeding uses `HasData` (part of the model/migration) instead of a runtime seeder — keeps
  seed data versioned with the schema.
- Repositories call `SaveChangesAsync` themselves (no separate `IUnitOfWork`) to keep the
  first lab slice small; revisitable if transactions spanning repositories are needed.
- `AddInfrastructure` takes a `string connectionString` (not `IConfiguration`) so the
  Infrastructure project stays free of configuration dependencies.

**Issue + fix**
- `dotnet ef migrations add` failed: *"startup project doesn't reference
  Microsoft.EntityFrameworkCore.Design"*. The `Design` package was `PrivateAssets=all` in
  Infrastructure, so it didn't flow to the Api startup project → added it to `Ecommerce.Api`.

**Verified:** build clean; migration created; CRUD + validation + 404 verified via `curl`;
`POST /api/orders` computed `1350.99`; **restart test confirmed persistence**.

## 2026-10-17 — Part B: shared UI + Blazor web host
- Added `HttpProductApiClient` to Infrastructure (implements `IProductApiClient`).
- Created `Presentation.Shared` (RCL) with `Components/Pages/Products.razor` — list + create/
  edit/delete, loading and error states.
- Created `Presentation.Web` (Blazor Web App, Interactive Server); registered
  `AddHttpClient<IProductApiClient, HttpProductApiClient>`; added a Products nav link;
  removed template Counter/Weather pages.

**Issue + fix**
- The shared `/products` route returned **404**. The component was in the RCL, but the
  Blazor Web App only discovers routable components from `AppAssembly` by default.
  Fixed by registering the assembly in **both** places:
  - `Routes.razor` → `Router.AdditionalAssemblies`
  - `Program.cs` → `MapRazorComponents<App>().AddAdditionalAssemblies(...)`

**Second issue:** an `EmitCompilerGeneratedFiles` probe left duplicate generated `.g.cs`
files in `obj/`, which broke the build with `CS0111/CS0102`. Fixed by deleting all
`obj`/`bin` folders and rebuilding.

**Verified:** `/products` → `200` and renders all six seeded products and the category
dropdown, fetched from the API/SQLite.

## Next
- Part C: add `Presentation.Client` (.NET MAUI Blazor Hybrid) referencing `Presentation.Shared`.
- Backfill Labs 1–4 into their own folders.
- Add xUnit tests for `ProductService` / `OrderService`.
