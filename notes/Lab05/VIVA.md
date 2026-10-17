# Viva Prep — Lab 05 (E-commerce + EF Core)

Short, confident answers. Read `IMPLEMENTATION.md` for the story behind each change.

## EF Core fundamentals

**What is EF Core and why use it here?**
An object-relational mapper: it maps C# classes to database tables so we write LINQ instead
of SQL. It gives us migrations (versioned schema), change tracking, and provider
independence. Lab 5 asks specifically for EF Core.

**Code-first vs database-first?**
We use **code-first**: entities and configurations define the model, and migrations generate
the schema. It suits a growing coursework project.

**What is a `DbContext`?**
The session with the database. It exposes `DbSet<T>` per aggregate and tracks changes.
Ours is `EcommerceDbContext`, registered as **scoped** so each request gets its own instance.

**What is a migration and what does `dotnet ef migrations add InitialCreate` do?**
A migration is a versioned C# snapshot of the schema change. `migrations add` compares the
model to the last snapshot and writes `Up`/`Down` methods. `Database.Migrate()` at API startup
applies pending migrations — that is why the database appears on first run.

**Why is decimal configured with `HasPrecision(18, 2)`?**
Money needs fixed scale. On SQLite decimals are stored as TEXT, so precision is declared
explicitly for correctness and portability.

**What is `HasData` and why?**
Seed data declared inside the model so it ships *in the migration*. Categories and products
are therefore versioned with the schema instead of seeded ad-hoc at runtime.

**How are relationships modelled?**
`Category 1—* Product` via `CategoryId`/`Category` (`DeleteBehavior.Restrict`), and
`Order 1—* OrderItem`, plus `Product 1—* OrderItem`. Configured with the Fluent API in
`Infrastructure/Persistence/Configurations`.

## Clean architecture

**Why does the Domain have no project references?**
It holds business rules only. Keeping it dependency-free makes it testable and framework
independent — the core of clean architecture.

**Where does the database live and why?**
In `Infrastructure` (server side), used by `Ecommerce.Api`. The Domain/Application layers
only see interfaces (`IProductRepository`), so the provider is swappable.

**Why not inject the `DbContext` straight into a Razor component?**
That would leak persistence into the UI and break the dependency rule. UI → `IProductApiClient`
→ API → Application → Infrastructure.

**Why do repositories call `SaveChanges` instead of a Unit of Work?**
Deliberate simplification for this lab. Each operation is a single aggregate change, so a
per-repository save keeps the code small. A Unit of Work is the natural next step if a use
case must span multiple repositories atomically.

## API & data

**How is data validated?**
In the Application services (`ProductService.Validate`, `OrderService`). Invalid input throws
`ArgumentException`, which the endpoints translate to **400** with `{ "error": "..." }`;
missing resources return **404**. Validation lives in the use case, not the controller.

**What proves the data is actually persisted (not just in memory)?**
Stop the API, restart it, and the created order is still there while the deleted product
stays gone — because SQLite is writing to `ecommerce.dev.db` on disk.

**Why SQLite?**
Zero setup, runs identically in Codespaces (Linux) and Windows, and echoes the course's
SQLite theme. Swapping to SQL Server is one change in `AddInfrastructure` plus new migrations.

## Hosts

**Why two presentation hosts?**
MAUI Blazor Hybrid cannot build or run on Linux, but development happens in Codespaces.
All components live in `Presentation.Shared`; `Presentation.Web` previews them on Linux and
`Presentation.Client` (MAUI) will ship the same components on Windows. UI depends only on
`IProductApiClient`, so both hosts register the same abstraction.

## Possible examiner curveballs
- *"Show me the SQL."* Enable EF logging, or inspect `Migrations/*.cs` for the generated DDL.
- *"What if two requests delete the same product?"* `DeleteAsync` returns false → 404; SQLite
  serialises writes; concurrency tokens are a future improvement.
- *"Where would transactions matter?"* Creating an order touches `Orders` + `OrderItems` in
  one `SaveChangesAsync`, which EF wraps in a transaction.
- *"How would you add a new entity?"* Entity in Domain → configuration + `DbSet` in
  Infrastructure → migration → DTOs/service in Application → endpoints.
