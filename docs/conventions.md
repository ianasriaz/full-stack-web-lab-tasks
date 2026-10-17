# Conventions

## Code
- **Nullable reference types** and **implicit usings** are on everywhere.
- One public type per file; file name matches the type name.
- Prefer `sealed` classes for services, repositories and components.
- Async methods take a `CancellationToken` and end with `Async`.
- Use `record` for DTOs and immutable data; plain classes for EF entities.
- Keep methods small; a method should read like the use case it implements.
- No comments that restate the code. Comment only non-obvious decisions.

## Naming
- Projects: `Domain`, `Application`, `Infrastructure`, `Ecommerce.Api`, `Presentation.*`.
- Namespaces mirror the project (`Ecommerce.Domain`, `Ecommerce.Application.Services`, …).
- Tests (later): `<TypeUnderTest>Tests`.

## Layer rules
- `Domain` references nothing.
- `Application` references only `Domain`.
- `Infrastructure` implements Application interfaces using EF Core.
- `Api` and `Presentation.*` depend inward; they never reach into EF Core directly.

## HTTP / API
- Routes are lowercase, plural, resource-first: `/api/products`, `/api/products/{id}`.
- Return DTOs, never entities.
- `400` for validation errors (with `{ "error": "..." }`), `404` for missing resources.

## Git & documentation
- Commit messages: `type: summary` (Conventional Commits) — e.g. `feat(lab05): add EF Core SQLite persistence`.
- Every change to a lab updates that lab's `IMPLEMENTATION.md`.
- Architecture decisions are recorded as ADRs in `docs/adr/NNNN-title.md`.
