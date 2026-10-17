# ADR 0001 — Use Clean Architecture

- **Status:** Accepted
- **Date:** 2026-10-17
- **Lab:** 05

## Context
The lab requires an e-commerce application with a real database. The code must be easy to
extend across later labs and easy to defend in a viva. We could have written a single
project with everything mixed together.

## Decision
Split the solution into `Domain`, `Application`, `Infrastructure`, and outer layers
(`Ecommerce.Api`, `Presentation.*`), with dependencies pointing inward.

## Consequences
- ✅ Business rules are independent of EF Core, the database, and the UI.
- ✅ Layers can be tested and replaced in isolation.
- ✅ Matches the structure of the instructor's manuals.
- ⚠️ More projects and more indirection than a single-file demo — accepted on purpose.
