# Tech Stack

Single source of truth for what this project is allowed to use.

| Concern | Choice | Notes |
|---------|--------|-------|
| Runtime | **.NET 10** (`net10.0`) | LTS |
| Language | **C# 13** | Nullable + implicit usings enabled |
| API | **ASP.NET Core Minimal API** | Endpoint groups per resource |
| ORM | **Entity Framework Core 10.0.13** | Code-first, migrations |
| Database | **SQLite** | File `ecommerce.dev.db`, zero setup |
| UI framework | **Blazor** | Razor Class Library shared by two hosts |
| Web host | **Blazor Web App** (Interactive Server) | Runs on Linux / Codespaces |
| Desktop/mobile host | **.NET MAUI Blazor Hybrid** | Windows (follow-up) |
| API docs | **Swashbuckle.AspNetCore 10.3.0** | Swagger UI at `/swagger` |
| EF tooling | **dotnet-ef 10.0.13** | Global tool |

## Packages per project

| Project | Packages |
|---------|----------|
| `Domain` | _(none)_ |
| `Application` | _(none)_ |
| `Infrastructure` | `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design` |
| `Ecommerce.Api` | `Swashbuckle.AspNetCore`, `Microsoft.EntityFrameworkCore.Design` (for `dotnet ef`) |
| `Presentation.Shared` | `Microsoft.AspNetCore.Components.Web` |
| `Presentation.Web` | _(framework only)_ |

## Adding something new
A new package/provider/framework requires an ADR in `docs/adr/` explaining the trade-off.
This keeps the "interview approach" honest and makes the viva easy to defend.
