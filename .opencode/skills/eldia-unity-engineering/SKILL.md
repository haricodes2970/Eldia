---
name: eldia-unity-engineering
description: Use when changing ELDIA Unity scenes, prefabs, assets, packages, input, C# scripts, rendering settings, compilation, Play Mode, or build verification.
compatibility: opencode
---

# ELDIA Unity Engineering

## Core rule
Inspect first. Preserve installed Unity version, current render pipeline, package set, serialized references, and existing project conventions. Make the smallest compatible change.

## Before editing
- Confirm this is a Unity project and inspect `Assets/`, `Packages/`, `ProjectSettings/`, asmdefs, scenes, prefabs, and `.meta` files.
- Read Unity version, render pipeline, input backend, package versions, and relevant `.gitignore` rules from files; never infer them.
- Check Git status and avoid files with unknown or teammate changes.

## Implementation constraints
- Reuse configured Input System, packages, scenes, prefabs, ScriptableObjects, and event conventions.
- Do not upgrade Unity, change render pipeline, add packages, or introduce dependencies unless explicitly approved and demonstrably necessary.
- Keep C# changes small, serialized-field compatible, and consistent with existing namespaces and assembly boundaries.
- Preserve Unity GUIDs, `.meta` files, asset references, scene wiring, prefab variants, and serialized data.
- Avoid broad reformatting, generated-file churn, and edits outside requested scope.

## Verification
- Check compilation in installed Unity version; inspect Console for errors and warnings relevant to changes.
- Test affected scene(s), input path, serialization, and Play Mode behavior when Unity is available.
- For builds, verify target, output path, clean/known-good state, and reproducible command or Editor steps.
- Report Inspector wiring explicitly: object, component, field, assigned asset/reference, and manual setup.
- Separate **tested results** from **unverified assumptions**. Never present an assumption as a test result.

## Handoff
List changed files, Unity version tested, scenes/assets touched, verification commands or steps, known limitations, and required manual Editor setup. Do not begin gameplay implementation during Phase 0.

## Red flags
Stop before editing if Unity version, pipeline, input backend, ownership of changed files, or required serialized references are unclear. Ask instead of guessing.
