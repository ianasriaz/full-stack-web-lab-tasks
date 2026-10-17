# AGENTS.md — How to work in this repository

This file orients any agent (or human) in ~2 minutes. Read it before touching code.

## What this repo is
Coursework for **BSIT Full Stack Web Development Lab**. The project is an **e-commerce
application** that is grown lab by lab. Each lab is a **self-contained folder** with its
own solution so it can be submitted and defended independently.

## Non-negotiables
1. **Follow clean architecture.** `Domain` → `Application` → `Infrastructure`; UI and API
   depend inward. See `docs/architecture.md`.
2. **Only the documented tech stack.** See `docs/tech-stack.md`. Do not introduce a new
   framework, ORM, or database without an ADR.
3. **One lab = one folder.** Never mix two labs in one folder.
4. **Submission folders stay clean.** A lab folder contains **only the solution and its
   source** — no markdown, no notes. See "Submission hygiene" below.
5. **Document as you go.** Every change updates the lab's notes (see below). Significant
   decisions get an ADR in `docs/adr/`.
6. **Interview → Plan → Approve → Implement.** Do not dump large amounts of code without a
   plan first. See `docs/vibe-coding-workflow.md`.
7. **Verify before claiming done.** Build, run, and hit the endpoints. Record proof.

## Submission hygiene (important)
The lab folders are handed directly to the instructor, so they must look like ordinary
student work:

- **Nothing but the solution** goes inside `<LabNN-Topic>/` — just `<Solution>.slnx` + `src/`.
- **No coursework markers** inside the submission: avoid the words *Lab, Task, Viva,
  Implementation* in file names, folder names, project names, UI text and code comments.
- **All preparation material lives in `notes/`** — viva prep, implementation logs, the
  build story, recorded proof. `notes/` is never submitted.
- Generated artifacts (`bin/`, `obj/`, `*.db`) are gitignored and should be deleted before
  handing the folder over.

## Folder conventions
```
<root>/
├── AGENTS.md
├── docs/                     # shared docs + ADRs (not submitted)
├── notes/                    # per-lab prep material (not submitted)
│   ├── README.md             # lab → submission-folder mapping
│   └── LabNN/                # README.md · IMPLEMENTATION.md · VIVA.md
├── Lab Manuals/              # instructor material (read-only reference)
└── LabNN-<Topic>/            # ★ SUBMITTED — solution only, no markdown
    ├── <Solution>.slnx
    └── src/                  # projects
```

## Where things live (Lab 05)
| Concern | Project |
|---------|---------|
| Entities | `src/Domain` |
| Use cases, DTOs, interfaces | `src/Application` |
| EF Core `DbContext`, migrations, repositories | `src/Infrastructure` |
| REST API | `src/Ecommerce.Api` |
| Shared Blazor UI (used by both hosts) | `src/Presentation.Shared` |
| Blazor web host (Linux/Codespaces preview) | `src/Presentation.Web` |
| MAUI Blazor Hybrid host (Windows) | `src/Presentation.Client` *(follow-up)* |

## Commands
```bash
cd Lab05-EFCore-Database
dotnet build Ecommerce.slnx
dotnet ef migrations add <Name> -p src/Infrastructure -s src/Ecommerce.Api
dotnet run --project src/Ecommerce.Api --no-launch-profile
dotnet run --project src/Presentation.Web --no-launch-profile
```

## Gotchas learned here
- `dotnet-ef` must be installed: `dotnet tool install --global dotnet-ef`.
- The **startup project** of migrations needs `Microsoft.EntityFrameworkCore.Design`.
- The MAUI project does not build on Linux; in Codespaces only run the API/web hosts.
- A Blazor Web App only discovers routable components in other assemblies if the assembly
  is registered in **both** `Router.AdditionalAssemblies` *and*
  `MapRazorComponents<App>().AddAdditionalAssemblies(...)`.
- Never use `pkill -f "<ProjectName>"` in this shell — it matches and kills the running
  command itself. Use `kill_shell` instead.
