# Vibe Coding Workflow

Our agreed way of working. The goal is **small, reviewable, documented steps** so that
(a) any agent can pick the project up cold, and (b) every decision is defensible in a viva.

## The loop

```
1. INTERVIEW  ┌─ We discuss the task. I ask about scope, stack, ambiguity.
              │  No code is written yet.
2. PLAN       │  I write a plan as a Markdown file and list the trade-offs.
              │  → You approve (or refine) before implementation starts.
3. IMPLEMENT  │  Smallest correct increment. One concern at a time.
              │  Talk before big code dumps.
4. DOCUMENT   │  Update the lab's IMPLEMENTATION.md and add an ADR for decisions.
              │  Docs are written *as we go*, never bolted on at the end.
5. VERIFY     │  Build → run → hit the endpoints → record proof.
              │  Never claim "done" without evidence.
6. COMMIT     │  One logical change per commit, Conventional Commits message.
```

## Why the documentation files exist

| File | Purpose | Who reads it |
|------|---------|--------------|
| `AGENTS.md` | Onboarding: stack, rules, where things live. | Any agent or human, first. |
| `docs/architecture.md` | The mental model of the system. | Viva examiner, new contributor. |
| `docs/adr/*` | *Why* a decision was made. | Viva defense. |
| `LabNN/README.md` | The task, the deliverable, how to run it. | Instructor. |
| `LabNN/IMPLEMENTATION.md` | Chronological log of what changed and why. | You, when you forget. |
| `LabNN/VIVA.md` | Likely questions + crisp answers. | You, the night before. |

## Anti-patterns we avoid
- Dumping a whole feature and writing docs afterwards.
- Skipping verification ("it should work").
- Introducing a package or pattern without an ADR.
- Mixing two labs in one folder.
