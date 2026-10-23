# ELDIA Theme Integration Plan

Status: planning document only. No gameplay implementation is included here.

## Official brief

- Theme: Cyber Intrusion
- Problem statement: The Matrix — Break the Simulation
- Theme concepts: Reality, Technology, Deception
- Suggested mechanics: Reality glitching and dimension swapping, cybernetic terminal hacking puzzles, stealth ICE countermeasures, surveillance evasion

## Current playable-slice pitch

ELDIA is a compact 2D cyberpunk action slice in which an infiltrator enters a
simulated district, disables its control node, and defeats the District Warden,
a security guardian protecting the simulation. The current prototype already
supports movement, directional attacks, an interactive objective, boss combat,
and a two-condition mission victory. The theme-aligned presentation should
make those existing actions read clearly as a cyber intrusion without expanding
this prototype into a complete 3D RPG or a large systemic stealth game.

## Before and after

### Verified current prototype

The current runtime builder is:

```text
Assets/ELDIA/Scripts/Core/PlayerTestArena.cs
```

It generates:

- A bounded top-down arena with floor, walls, and obstacles.
- A player with `PlayerController2D`, `PlayerAttack`, and health.
- A stationary ordinary enemy.
- A `SimulationObjective` control node.
- A `BasicBoss` District Warden.
- A `MissionCoordinator`.
- A camera, temporary IMGUI HUD, and local `R` restart control.

Current mission flow:

```text
Intro -> Playing
Objective completed + boss defeated
Playing -> Victory
```

The current source does not implement hacking puzzles, stealth, surveillance
systems, ICE countermeasures, reality swapping, dimension swapping, or a
simulation-collapse effect. The current arena uses runtime-generated colored
sprites and built-in Unity components rather than a finished cyberpunk art
set.

### Proposed theme-aligned slice

Keep existing contracts and gameplay ownership. Reframe the current objective
as a cybernetic terminal breach, the boss as the simulation's security
guardian, and victory as a controlled simulation collapse. Add visual and HUD
feedback around those existing events before considering new mechanics.

The first theme pass should change player understanding and presentation more
than core rules:

- The cyan simulation node becomes an intrusion terminal.
- `E` interaction communicates breach/disable instead of generic completion.
- The District Warden communicates active security response.
- Objective and boss status become security/breach status in HUD messaging.
- Victory feedback communicates that the district simulation has collapsed.
- Environment work adds restrained artificial-world corruption around the
  existing level blockout.

## Proposed player journey

1. **Infiltrate district**
   - Player enters the simulated district.
   - Movement, walls, obstacles, and the camera establish the playable space.
   - Existing ordinary enemy provides the first security threat.

2. **Reach simulation node**
   - Player navigates to the visible control terminal.
   - HUD or an interaction prompt identifies the terminal as the intrusion
     target.

3. **Breach or disable node**
   - Player presses `E` within the existing interaction range.
   - Existing `IObjective.TryComplete()` flow marks the terminal disabled.
   - Feedback should say that the simulation control layer is breached.

4. **Defeat security guardian**
   - Player uses existing directional `Fire1` attacks against the District
     Warden.
   - Boss defeat remains published through `IBoss.Defeated`, not generic
     entity defeat.

5. **Trigger simulation collapse and victory**
   - Existing `MissionCoordinator` observes both authoritative conditions:
     `IObjective.IsCompleted` and `IBoss.IsDefeated`.
   - `Playing -> Victory` remains the single mission transition.
   - Presentation responds to `VictoryTriggered` with a short simulation-
     collapse message/effect.

The order of terminal breach and guardian defeat may remain flexible because
current mission rules require both conditions, not a fixed event order.

## Minimal vertical-slice priorities

### P0 — required for theme-aligned playable build

1. **Clear theme framing**
   - Name the slice as a cyber intrusion into a simulated district.
   - Use consistent terminal, security, breach, and collapse terminology.

2. **Functional terminal interaction**
   - Preserve `SimulationObjective` and `ObjectiveInteractor`.
   - Make the interaction prompt and completion feedback unambiguous.
   - Keep objective ID and event contract stable.

3. **Readable combat**
   - Preserve `PlayerAttack`, `IDamageable`, `HealthComponent`, and existing
     boss defeat flow.
   - Ensure ordinary enemy and District Warden are visually distinct.
   - Keep hit, damage, and defeat feedback readable.

4. **Coherent victory transition**
   - Preserve `MissionCoordinator` as authority.
   - Show no victory before both objective completion and boss defeat.
   - Present victory as simulation collapse only after `VictoryTriggered`.

### P1 — high-value presentation improvements

1. **Visual glitches**
   - Brief controlled color shifts, scanline-like disruption, sprite flicker,
     or restrained distortion around terminal completion and victory.
   - Effects must not hide hitboxes, attack telegraphs, or status text.

2. **Corrupted environment details**
   - Add a small number of neon/corrupted-digital markers to the existing
     blockout: broken grid fragments, warning panels, or simulated geometry
     seams.
   - Preserve collision readability and keep art assets within the team's
     agreed ownership.

3. **HUD breach feedback**
   - Add objective prompt, breach status, security-alert status, boss status,
     and simulation-collapse feedback through existing event contracts.
   - HUD must observe mission/objective/boss state rather than own duplicates.

### P2 — optional, risk-gated mechanics

1. **Stealth / ICE countermeasures**
   - Consider only if a small, testable mechanic can be defined without
     replacing current combat or mission flow.
   - Do not begin with a full detection, patrol, or stealth framework.

2. **Dimension or reality shifting**
   - Consider only as a bounded presentation or traversal modifier after P0
     is stable.
   - Avoid creating parallel level state, duplicate physics worlds, or a second
     mission manager.

3. **Terminal hacking puzzle**
   - Consider only if it can remain a small extension of the current objective
     interaction rather than a new UI or puzzle framework.

P2 items are not required for the first gameathon slice. A simple complete
mission is preferable to several unfinished mechanics.

## Ownership boundaries

### Hari — core integration

Owns:

- Core runtime integration.
- Mission flow and shared contracts.
- PlayerTest arena ownership and final scene integration.
- Unity compilation, scene validation, and final verification.
- Guarding against duplicate systems and incompatible branch changes.

Hari should not silently absorb teammate-owned presentation, combat-AI, or
level-art systems into core contracts.

### Chetan — combat and security behavior

Owns:

- Katana/combat implementation.
- Ordinary enemy AI.
- Akagetsu or future boss attack behavior.
- Combat feedback that belongs to combat entities.

Chetan should reuse `DamageRequest`, `DamageResult`, `IDamageable`, `IHealth`,
`IBoss`, and existing defeat events. Do not create a second health or damage
system.

### Pannaga — environment and level presentation

Owns:

- Environment blockout and collision layout.
- Cyberpunk district presentation.
- Parallax and visual corruption.
- Art assets and level-authored presentation.

Pannaga should coordinate with `PlayerTestArena` ownership before replacing
runtime-generated geometry or introducing serialized scene objects. Collision
changes must be checked against the player controller and attack query.

### Divish — HUD and mission presentation

Owns:

- HUD presentation.
- Terminal interaction prompts.
- Boss health/status display.
- Mission victory/defeat visuals.
- Breach and simulation-collapse messaging.

Divish should consume `IObjective`, `IBoss`, `IGameState`, and
`IVictoryState`. HUD must not create a duplicate authoritative mission state.

## Existing integration contracts

Relevant paths:

```text
Assets/ELDIA/Scripts/Gameplay/Shared/
Assets/ELDIA/Scripts/Gameplay/Mission/MissionCoordinator.cs
```

Important existing boundaries:

- `IObjective.Completed` and `IObjective.IsCompleted` describe terminal
  completion.
- `IBoss.Defeated` and `IBoss.IsDefeated` describe boss completion.
- `IHealth.Defeated` is generic and must not be used as a boss-specific event.
- `IDamageable.ReceiveDamage(DamageRequest)` owns damage application.
- `DamageResult.AmountApplied` is actual health lost.
- `MissionCoordinator` is the authority for `Playing -> Victory`.
- `ArenaHUD` observes existing events and state.

Full signatures and current responsibilities are documented in:

```text
Docs/TEAM_INTEGRATION_CONTRACT.md
```

No new shared event, objective ID, boss ID, mission manager, or victory rule is
proposed for P0.

## Dependencies and branch integration risks

### Dependencies

- Chetan combat work must preserve existing damage and defeat contracts.
- Pannaga environment work must preserve player collision and camera needs.
- Divish HUD work must consume existing mission/objective/boss events.
- Hari must integrate scene ownership and run Unity verification after branch
  integration.

### Risks

1. **Runtime scene ownership**
   - `PlayerTestArena` generates the current test arena at runtime.
   - A teammate branch must not add duplicate player, camera, boss, objective,
     mission, or HUD systems to the same test scene.

2. **Generated versus serialized level content**
   - Replacing generated geometry with serialized environment objects changes
     ownership and cleanup assumptions. Integrate that deliberately.

3. **Shared contract drift**
   - Changing event payloads, IDs, or victory conditions can break mission,
     HUD, and teammate branches. Prefer adapters or implementation changes
     before changing shared declarations.

4. **Input mapping**
   - Legacy Input Manager remains active.
   - Current `Fire1` settings are authoritative; the existing HUD/comment
     mentions `Ctrl/J`, but project settings currently map `Fire1` to Left Ctrl
     and mouse button 0. Do not silently change mappings in a feature branch.

5. **Visual effects and readability**
   - Glitches and corruption must not obscure attacks, objective state, or
     collision boundaries.

6. **Recovery and metadata safety**
   - Preserve Unity `.meta` files and GUIDs.
   - Preserve local recovery evidence at
     `Logs/meta-backup-20261023-170303/`.
   - Do not include unapproved `ProjectSettings/PackageManagerSettings.asset`.

## Implemented, proposed, and unverified

### Implemented and verified from source

- Runtime-generated top-down arena.
- Player movement at configured speed `5` units/second.
- Directional `Fire1` attack and damage contracts.
- Stationary ordinary enemy.
- Interactive simulation control node using `E` and 1.5-unit range.
- District Warden boss with boss-specific defeat event.
- Mission coordinator requiring objective completion and boss defeat.
- Built-in IMGUI HUD with objective, boss, state, and victory messaging.
- Local `R` scene restart behavior.
- Editor scene and runtime-ownership validators.

"Verified from source" means declarations and control flow were inspected; it
does not mean Play Mode behavior has been verified.

### Proposed by this plan

- Terminal/intrusion framing and interaction messaging.
- Security-alert and breach HUD language.
- Simulation-collapse victory presentation.
- P1 glitch and corrupted-district presentation.
- Optional P2 stealth/ICE or bounded reality-shift mechanics.
- Theme-aligned environment details and final presentation pass.

### Not currently implemented or verified

- Hacking puzzle system.
- Reality glitching or dimension swapping.
- Stealth detection or ICE countermeasures.
- Surveillance evasion.
- Enemy AI patrol/detection behavior.
- Final cyberpunk art pipeline.
- Play Mode behavior for the full journey.
- Teammate branch compatibility.

## Acceptance criteria for first theme-aligned playable build

A build is acceptable when all P0 criteria are met:

1. Player can enter the district and understand the cyber-intrusion objective
   without external explanation.
2. Terminal is visibly identifiable and has a clear interaction prompt.
3. Pressing `E` nearby disables/breaches the terminal exactly once.
4. Combat is readable and the security guardian can be defeated with existing
   attack/damage flow.
5. HUD clearly communicates terminal state, guardian state, and mission state.
6. Victory appears only after both terminal completion and guardian defeat.
7. Victory presentation communicates simulation collapse.
8. Restart returns the test slice to a fresh mission state.
9. No duplicate player, camera, objective, boss, mission, or HUD systems appear
   after repeated scene entry/restart testing.
10. Unity compilation and editor validators pass without new errors.
11. Manual Play Mode verification records movement, combat, interaction, boss,
    victory, restart, and repeated-entry results.
12. No new package dependency or incompatible input backend is introduced.

## Milestones

### Milestone 1 — narrative and terminology lock

- Confirm terminal, guardian, breach, security, and collapse language.
- Keep current IDs, contracts, and mission conditions unchanged.
- Agree on teammate ownership boundaries.

### Milestone 2 — P0 interaction and mission presentation

- Improve terminal prompt and completion feedback.
- Improve security/boss status language.
- Add a clear simulation-collapse victory message/effect.
- Keep `MissionCoordinator` authoritative.

### Milestone 3 — P0 runtime verification

- Compile in Unity.
- Run editor and ownership validators.
- Manually test the complete player journey.
- Record failures before adding new mechanics.

### Milestone 4 — P1 environment and glitch pass

- Add restrained corrupted-district details.
- Add readable terminal/breach and victory glitch effects.
- Verify effects do not obscure gameplay.

### Milestone 5 — integration checkpoint

- Integrate teammate branches in small batches.
- Recheck contracts, metadata, scene ownership, compilation, and Play Mode.
- Preserve the working P0 slice as fallback.

### Milestone 6 — optional P2 decision

- Evaluate remaining time and risk.
- Add at most one bounded stealth/ICE or reality-shift experiment if P0/P1
  remain stable.
- Otherwise ship the complete, readable intrusion mission without P2.

## Open decisions

- Should the terminal require a short multi-step breach interaction, or should
  the current single `E` completion remain the P0 behavior?
- Should the first security response be visual-only, or should Chetan provide a
  bounded enemy/boss behavior change?
- Which P1 glitch effect can be implemented without new packages or fragile
  rendering changes?
- Will Pannaga preserve runtime-generated blockout ownership or transition part
  of the arena to serialized level assets?
- Which exact teammate branch changes are ready for integration?

These decisions should be resolved by smallest playable increment, not by
expanding shared contracts prematurely.

## Scope statement

This plan translates the official Cyber Intrusion brief into the current ELDIA
prototype's existing objective, combat, mission, and presentation boundaries.
It does not claim that the suggested hacking, stealth, surveillance, or reality-
shifting mechanics already exist. No gameplay implementation or Play Mode
verification was performed while creating this document.
