# Full Stack Web Development Lab — E-commerce Project

This repository holds my **BSIT Full Stack Web Development Lab** work. The running
example is an **e-commerce application** built incrementally, lab by lab, using clean
architecture.

> **Read `AGENTS.md` first.** It is the onboarding document for both humans and AI agents.

## Labs

| Lab | Folder | Status | Topic |
|-----|--------|--------|-------|
| Lab 05 | [`Lab05-EFCore-Database/`](./Lab05-EFCore-Database/) | ✅ Implemented | Add a database to the e-commerce app with **EF Core** + SQLite |
| Lab 01–04 | _to be backfilled_ | ⏳ Planned | MAUI Blazor scaffolding, REST API consumption, local storage |

Each lab lives in its own self-contained, submittable folder with its own solution and docs.

## Tech stack

.NET 10 · ASP.NET Core Minimal API · EF Core + SQLite · Blazor (Razor Class Library) ·
.NET MAUI Blazor Hybrid (Windows head) · Swagger.

See [`docs/tech-stack.md`](./docs/tech-stack.md) for exact versions.

## Repository layout

```
.
├── AGENTS.md                 # start here
├── docs/                     # cross-cutting documentation + ADRs (not submitted)
├── notes/                    # per-lab prep material: README · IMPLEMENTATION · VIVA (not submitted)
├── Lab Manuals/              # instructor / course manuals (reference)
└── Lab05-EFCore-Database/    # ★ submitted folder — solution + src only
```

> **Submission hygiene:** lab folders are handed to the instructor directly, so they contain
> only the solution and source. Everything else (viva prep, implementation logs, how-to-run
> notes) lives in `notes/`. See `AGENTS.md`.

## Quick start (Lab 05)

```bash
cd Lab05-EFCore-Database

# 1. Run the API (applies migrations + seeds SQLite)
dotnet run --project src/Ecommerce.Api --no-launch-profile

# 2. In a second terminal, run the Blazor web preview
dotnet run --project src/Presentation.Web --no-launch-profile
```

API: <http://localhost:5088> (Swagger at `/swagger`) · Web UI: <http://localhost:5099/products>
