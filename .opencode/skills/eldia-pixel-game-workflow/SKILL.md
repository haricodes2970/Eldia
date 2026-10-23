---
name: eldia-pixel-game-workflow
description: Use when designing or implementing ELDIA 2D/2.5D pixel-art gameplay, sprites, tilemaps, cameras, animation, parallax, lighting, particles, glitch effects, or visual performance work.
compatibility: opencode
---

# ELDIA Pixel Game Workflow

## Core rule
Build a compact, readable 2D/2.5D pixel game. Core gameplay loop and stable frame rate come before visual complexity or asset volume.

## Pixel consistency
- Inspect existing project conventions before changing art settings.
- Keep sprite resolution, Pixels Per Unit, reference resolution, camera projection/scale, filter mode, compression, and mipmap choices consistent.
- Use tilemaps, colliders, sorting layers, sorting groups, and Animator Controllers appropriate to 2D/2.5D.
- Preserve existing assets, `.meta` files, GUIDs, palette decisions, and provenance.

## Animation and effects
- Prefer limited-frame idle, movement, attack, hit, and death animations when needed; avoid animation work without gameplay value.
- Use layered parallax, restrained neon accents, particles, and controlled glitch effects that preserve combat readability.
- Keep foreground, gameplay, and background sorting explicit; avoid visual effects obscuring collision or telegraphs.
- Use placeholder art and simple effects until the loop is playable.

## Performance and scope
- Profile representative gameplay; target stable frame pacing for the sprint target.
- Avoid unnecessary imported/generated assets, runtime allocations, excessive particles, and per-frame searches.
- Do not add art tools, packages, shaders, or assets during setup unless explicitly approved.
- Record asset license, source, author, and modification status for every third-party asset.

## Verification
Test camera pixel stability, sprite filtering, animation transitions, tilemap collision, sorting, parallax behavior, and effects under representative gameplay. State what was tested and what remains unverified. Report required Inspector setup and scene wiring.
