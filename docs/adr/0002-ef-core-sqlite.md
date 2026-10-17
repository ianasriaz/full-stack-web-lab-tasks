# ADR 0002 — EF Core with SQLite for the database

- **Status:** Accepted
- **Date:** 2026-10-17
- **Lab:** 05

## Context
Lab 5 asks to "add a database" and to use **EF Core** for database operations. The
development environment is GitHub Codespaces (Linux) plus a Windows laptop. The previous
lab used a client-side `sqlite-net-pcl` file; Lab 5 moves the database to the server.

## Decision
Use **Entity Framework Core (code-first)** with the **SQLite** provider, configured in the
`Infrastructure` project and consumed by the `Ecommerce.Api` project. Schema is managed
with **migrations**; reference data (categories, products) is seeded via `HasData`.

## Rationale
- SQLite is file-based: it runs identically in Codespaces and on Windows with no service
  to install — ideal for a deadline and for reproducible marking.
- It echoes the course's existing SQLite theme while introducing EF Core properly.
- The provider is an Infrastructure detail, so swapping to SQL Server/PostgreSQL later is
  a change to one method (`AddInfrastructure`) plus new migrations.

## Consequences
- ✅ Zero-setup database; the app creates `ecommerce.dev.db` on first run.
- ✅ Migrations give a real, revertible schema history to demonstrate.
- ⚠️ SQLite has weaker concurrency and limited decimal ordering; acceptable for this lab.
