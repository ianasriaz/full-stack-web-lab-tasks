# ADR 0003 — Shared Blazor UI with two hosts

- **Status:** Accepted
- **Date:** 2026-10-17
- **Lab:** 05

## Context
The manuals target **.NET MAUI Blazor Hybrid**, which only builds and runs on
Windows/Android/Mac. Development, however, happens in a **Linux Codespaces container**,
where MAUI cannot run — so there would be no way to preview the UI while building it.

## Decision
Put all Razor components in a **Razor Class Library** (`Presentation.Shared`) and reference
it from two thin hosts:
1. `Presentation.Web` — a Blazor Web App (Interactive Server) that runs on Linux and is used
   for previewing/development.
2. `Presentation.Client` — the .NET MAUI Blazor Hybrid host used for the Windows delivery.

Both hosts talk to the API through the same `IProductApiClient` abstraction.

## Consequences
- ✅ One UI codebase, previewable in Codespaces and shippable as a MAUI app.
- ✅ Reinforces clean architecture: the UI depends only on Application abstractions.
- ⚠️ Two host projects to keep in sync (they only differ in service registration).
- ⚠️ Server-side DB access from the UI is impossible by design — everything goes through the API.
