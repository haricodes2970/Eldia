---
name: eldia-safe-integration
description: Use when integrating ELDIA gameplay, Unity assets, scenes, prefabs, scripts, branches, or teammate work, especially during the coordinated sprint.
compatibility: opencode
---

# ELDIA Safe Integration

## Core rule
Preserve unknown work. Integrate in small, reviewable batches with a known-good checkpoint.

## Git guardrails
- Inspect `git status`, branch, and diff before every integration batch.
- Keep Hari's work on `feat/hari-core`; do not silently create, switch, merge, rebase, or delete branches.
- Never commit teammates' work without explicit authorization.
- Never force-push, rewrite shared history, reset destructively, or delete unknown files.
- Preserve uncommitted changes and stop when ownership or conflict intent is unclear.
- Never commit generated Unity directories or files: `Library/`, `Temp/`, `Obj/`, `Logs/`, build output, or equivalent machine-generated artifacts.

## Unity asset safety
- Preserve `.meta` files, GUIDs, serialized references, scenes, prefabs, prefab variants, package lock files, and project settings.
- Do not trigger broad asset reimports, Unity upgrades, package changes, or scene rewrites without approval.
- Agree on shared interfaces, event names, serialized contracts, and ownership before integrating gameplay systems.

## Batch workflow
1. Capture baseline status and identify owned files.
2. Define one small integration batch and its acceptance check.
3. Apply only scoped changes; preserve unrelated work.
4. Compile/test the affected path and record failures separately from assumptions.
5. Review diff and status; confirm generated files are excluded.
6. Mark a known-good checkpoint only when the batch passes. Do not commit or push unless authorized.

## Stop conditions
Stop and ask when branch is wrong, status contains unexpected changes, a conflict affects teammate work, a `.meta`/GUID may be regenerated, an interface is undefined, or verification cannot distinguish old failures from new failures.

## Handoff
Report branch, baseline status, files changed, files intentionally untouched, tests run, known-good checkpoint, unresolved conflicts, and manual Unity/Inspector steps.
