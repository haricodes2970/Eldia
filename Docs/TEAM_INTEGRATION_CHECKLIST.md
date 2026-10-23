# ELDIA Team Integration Checklist

Status: read-only readiness review against local `feat/hari-core`.

Baseline:

```text
11f1ba83b28b12585fd50a8ac167b7eec4c13b08
```

Main scene:

```text
Assets/ELDIA/Scenes/PlayerTest.unity
```

This checklist distinguishes source-verified behavior from proposed theme work
and Play Mode checks that remain unverified.

## 1. Agreed vertical-slice scope

### P0 — required

- [ ] Player can infiltrate one bounded simulated cyberpunk district.
- [ ] Simulation control node is clearly presented as a cybernetic terminal.
- [ ] Player can reach and breach/disable terminal through existing interaction
      flow.
- [ ] Ordinary security threat and District Warden combat remain readable.
- [ ] District Warden can be defeated through existing damage contracts.
- [ ] Mission victory requires both terminal completion and boss defeat.
- [ ] Victory presentation communicates simulation collapse.
- [ ] Restart returns mission to a fresh state.
- [ ] No duplicate player, camera, objective, boss, mission, or HUD objects are
      created after repeated scene entry/restart testing.
- [ ] Unity compilation, scene validation, and runtime-ownership validation pass.
- [ ] Manual Play Mode test records movement, camera, combat, interaction, boss,
      victory, restart, and repeated-entry behavior.

### P1 — add after P0 is stable

- [ ] Restrained glitch effects around terminal breach and victory.
- [ ] Corrupted-digital/neon environment details that preserve readability.
- [ ] HUD breach/security-alert messaging and interaction prompt presentation.

### P2 — optional and time/risk gated

- [ ] One bounded stealth/ICE countermeasure experiment, if feasible.
- [ ] One bounded reality/dimension presentation or traversal experiment, if
      feasible.
- [ ] A small terminal puzzle only if it does not require a second UI/objective
      framework.

P2 is not required for the first complete gameathon slice.

## 2. Ownership and source dependencies

| Checklist area | Owner | Existing source/contracts | Integration boundary |
|---|---|---|---|
| Core runtime and final integration | Hari | `Assets/ELDIA/Scripts/Core/PlayerTestArena.cs`; `MissionCoordinator.cs`; shared contracts | Preserve runtime ownership, IDs, events, and victory rule |
| Player movement/camera | Hari | `PlayerController2D.cs`; `CameraFollow2D.cs` | Top-down movement; `moveSpeed = 5`; no jump; `Rigidbody2D` + `BoxCollider2D` |
| Damage and health | Hari/Chetan integration | `Shared/DamageTypes.cs`; `Shared/HealthContracts.cs`; `HealthComponent.cs` | Use `DamageRequest`, `DamageResult`, `IDamageable`, `IHealth`; no duplicate health store |
| Katana combat | Chetan | `PlayerAttack.cs`; `IDamageable`; `DamageRequest`; `DamageResult` | Preserve actual `AmountApplied` semantics and target validation |
| Ordinary enemy AI | Chetan | `BasicEnemy.cs`; `HealthComponent`; `IDamageable` | Existing enemy is stationary; AI may extend behavior without replacing health contract |
| Boss attacks/behavior | Chetan | `BasicBoss.cs`; `IBoss`; `BossDefeatedEvent`; `IDamageable` | Boss-specific defeat uses `IBoss.Defeated`; generic health defeat stays generic |
| Environment/blockout/parallax | Pannaga | `PlayerTestArena.cs`; `PlayerTest.unity`; current generated walls/obstacles | Coordinate before replacing runtime-generated geometry or scene ownership |
| Cyberpunk visual corruption | Pannaga | Current runtime sprite colors/sorting orders; scene ownership | Preserve collision readability and avoid hidden gameplay boundaries |
| Terminal interaction | Hari + Divish presentation | `SimulationObjective.cs`; `ObjectiveInteractor.cs`; `IObjective`; `ObjectiveCompletedEvent` | Keep objective ID `simulation-control-node`, `E`, and 1.5-unit range unless approved |
| HUD and prompts | Divish | `ArenaHUD.cs`; `IObjective`; `IBoss`; `IGameState`; `IVictoryState` | Observe authoritative state/events; do not create second mission manager |
| Victory/defeat presentation | Divish + Hari integration | `MissionCoordinator.cs`; `VictoryEvent`; `GameStateChangedEvent`; `ArenaHUD.cs` | Victory only after objective complete and boss defeated |
| Restart | Hari | `TestArenaReset.cs` | Local `R` scene reload; no shared restart-policy replacement |
| Final Unity verification | Hari | `PlayerTestSceneValidator.cs`; `PlayerTestRuntimeSourceValidator.cs` | Compile, validate scene/ownership, then manual Play Mode |

## 3. Existing contract checklist

### Combat and health

- [ ] Combat branch uses:

  ```csharp
  DamageResult ReceiveDamage(DamageRequest request);
  ```

- [ ] `DamageRequest.Amount` is requested damage.
- [ ] Target validates damage at application time.
- [ ] `DamageResult.AmountApplied` is actual health lost.
- [ ] Rejected damage returns `DamageResult.NotApplied`.
- [ ] `IHealth.HealthChanged` is used for health display/feedback.
- [ ] `IHealth.Defeated` remains generic `EntityDefeatedEvent`.
- [ ] Boss UI/mission code uses `IBoss.Defeated`, not generic health defeat.
- [ ] Boss ID remains `district-warden` unless Hari approves a contract change.

### Objective and mission

- [ ] Objective implementation remains `IObjective`.
- [ ] `ObjectiveCompletedEvent.ObjectiveId` remains authoritative for event
      identity, while `IObjective.IsCompleted` remains authoritative for state.
- [ ] Objective ID remains `simulation-control-node`.
- [ ] `ObjectiveInteractor` continues to call `TryComplete()` only within range.
- [ ] Mission starts with `Intro -> Playing`.
- [ ] Victory remains `Playing -> Victory`.
- [ ] Victory requires both:

  ```csharp
  objective.IsCompleted && boss.IsDefeated
  ```

- [ ] `VictoryTriggered` remains idempotent.
- [ ] Boolean arguments to `TryTriggerVictory(bool, bool)` are not treated as
      authoritative by consumers; live contract state is authoritative.

## 4. Integration order

### Step 0 — preserve core checkpoint

Hari verifies before each batch:

- [ ] Branch remains `feat/hari-core`.
- [ ] No unexpected files are overwritten.
- [ ] Current core reliability changes are reviewed separately.
- [ ] `ProjectSettings/PackageManagerSettings.asset` remains untracked unless
      explicitly approved.
- [ ] `Logs/meta-backup-20261023-170303/` remains preserved.

### Step 1 — Chetan combat branch

Branch target:

```text
feat/chetan-combat
```

Integrate combat implementation first because boss and enemy behavior depend on
stable damage/health semantics.

Checks before integration:

- [ ] Branch diff uses existing shared contracts or documents any required API
      proposal before changing them.
- [ ] No second health/damage system exists.
- [ ] Ordinary enemy and boss components remain attachable to generated/runtime
      objects.
- [ ] Boss still emits one `BossDefeatedEvent`.

Minimum verification after Chetan integration:

```text
Unity batch compilation
PlayerTestSceneValidator
PlayerTestRuntimeSourceValidator
Manual: attack ordinary enemy
Manual: damage and defeat boss
Manual: no duplicate defeat/victory events
```

### Step 2 — Pannaga environment branch

Branch target:

```text
feat/pannaga-level
```

Integrate environment/blockout after combat contracts remain stable. Decide
whether environment remains runtime-generated or transitions to serialized
scene ownership before merging broad scene edits.

Checks before integration:

- [ ] Player spawn and camera target remain valid.
- [ ] Walls/obstacles retain usable 2D colliders.
- [ ] Environment does not add a duplicate player, camera, objective, boss, or
      mission manager.
- [ ] Sorting and collision behavior remain readable.
- [ ] `.meta` files and GUIDs are preserved.

Minimum verification after Pannaga integration:

```text
Unity batch compilation
PlayerTestSceneValidator
PlayerTestRuntimeSourceValidator, if runtime ownership remains active
Manual: player movement and camera follow
Manual: wall/obstacle collision
Manual: attack range and visibility
Manual: no duplicate camera or arena owner
```

### Step 3 — Divish UI branch

Branch target:

```text
feat/divish-ui
```

Integrate presentation after mission/objective/boss event flow is stable. UI
must observe the existing contracts rather than become a second state owner.

Checks before integration:

- [ ] Objective prompt reads from objective state/event flow.
- [ ] Boss status reads from `IHealth`/`IBoss` state as appropriate.
- [ ] Victory presentation subscribes to `VictoryTriggered`.
- [ ] No victory presentation appears before both prerequisites.
- [ ] Existing HUD is intentionally replaced/extended, not duplicated.
- [ ] No new package or incompatible input backend is introduced.

Minimum verification after Divish integration:

```text
Unity batch compilation
PlayerTestSceneValidator
Manual: initial HUD/prompt
Manual: objective completion feedback
Manual: boss defeat feedback
Manual: victory only after both conditions
Manual: restart clears UI state
```

### Step 4 — final Hari integration

- [ ] Reconcile branch conflicts without reset/rebase shortcuts.
- [ ] Review shared API changes separately from feature changes.
- [ ] Run metadata/GUID audit.
- [ ] Run `git diff --check`.
- [ ] Run Unity batch compilation.
- [ ] Run both editor validators.
- [ ] Run full manual Play Mode checklist.
- [ ] Preserve a known-good P0 fallback.

## 5. Theme mismatch: implemented versus proposed

### Implemented in source

- Runtime-generated bounded top-down arena.
- Player movement and camera.
- Directional `Fire1` attack.
- Generic health/damage contracts.
- Stationary ordinary enemy.
- Interactive simulation control node using `E` within 1.5 units.
- District Warden boss and boss-specific defeat event.
- Mission victory gated by objective completion plus boss defeat.
- IMGUI HUD with objective, boss, state, and victory messaging.
- Local `R` scene reload.
- Runtime ownership and scene validators in the current uncommitted sprint.

These are source-verified. They are not equivalent to Play Mode verification.

### Proposed by theme plan, not implemented

- Explicit terminal/breach terminology and interaction prompt.
- Security-alert messaging.
- Simulation-collapse visual transition.
- Glitch effects.
- Corrupted digital environment details.
- Hacking puzzle.
- Stealth or ICE countermeasures.
- Surveillance evasion.
- Reality glitching or dimension swapping.
- Enemy patrol/detection AI.
- Final cyberpunk art pipeline.

### Known wording/configuration mismatch

`PlayerAttack.cs` comments and current HUD text mention `J`, but verified
`ProjectSettings/InputManager.asset` maps `Fire1` to Left Ctrl and mouse button
0. Do not change mappings during branch integration without Hari's decision.

## 6. Minimum verification matrix

| Change area | Required static/editor checks | Required manual Play Mode checks |
|---|---|---|
| Combat | Compile; metadata; scene validator | Attack, damage amount, enemy defeat, boss defeat |
| Boss | Compile; contract/event review | Hit feedback, defeat once, boss state |
| Environment | Compile; scene validator; metadata | Spawn, camera, wall collision, sorting/readability |
| Objective | Compile; contract review | Range, `E`, one completion, visual/prompt feedback |
| Mission | Compile; contract review | Intro/Playing, both-order completion, one victory |
| HUD | Compile; scene validator | Initial state, updates, victory gating, restart reset |
| Runtime ownership | Ownership validator | Repeat Play Mode, `R`, no duplicate roots/camera/events |
| Final build | Compile; both validators; diff review | Full P0 journey and Console review |

No batch Unity operation should run while GUI Editor or Play Mode uses the
project.

## 7. Hari approval decisions

Resolve before implementation or branch merge:

1. Keep terminal completion as one nearby `E` press for P0, or add a bounded
   multi-step breach interaction?
2. Should security response remain visual/HUD-only for P0, or should Chetan add
   bounded enemy/boss behavior?
3. Which single P1 glitch effect is feasible without fragile rendering changes?
4. Does Pannaga preserve runtime-generated arena ownership, or transition part
   of the level to serialized scene assets?
5. Should the existing IMGUI HUD be extended or replaced by Divish's presentation
   implementation?
6. Should the `J` documentation mismatch be corrected in comments/UI only, or
   should `Fire1` input mapping change?
7. Which teammate branch is ready for first integration?
8. Are any shared API changes genuinely necessary, or can adapters preserve the
   current contracts?
9. Is the first target only the P0 intrusion mission, with P1/P2 deferred until
   after manual verification?

## 8. Current core reliability and documentation status

Current uncommitted reliability changes:

```text
Assets/ELDIA/Editor/PlayerTestSceneValidator.cs
Assets/ELDIA/Scripts/Core/PlayerTestArena.cs
Assets/ELDIA/Scripts/Player/PlayerController2D.cs
Assets/ELDIA/Editor/PlayerTestRuntimeSourceValidator.cs
Assets/ELDIA/Editor/PlayerTestRuntimeSourceValidator.cs.meta
```

Reliability intent:

- Generated objects share an arena-owned `Generated Arena` root.
- Generated camera is parented to that root.
- Cleanup no longer globally searches/deletes `Main Camera`.
- Player facing indicator wiring uses `PlayerController2D.Configure`.
- Repeated initialization is guarded.
- Scene validator checks the arena bootstrap component.
- Runtime ownership validator checks source-level ownership/wiring invariants.

Current documentation files:

```text
Docs/TEAM_INTEGRATION_CONTRACT.md
Docs/THEME_INTEGRATION_PLAN.md
Docs/TEAM_INTEGRATION_CHECKLIST.md
```

The integration contract and theme plan are untracked local documentation at
this review point. `ProjectSettings/PackageManagerSettings.asset` is also
untracked and intentionally excluded.

## 9. Review outcome

Current readiness state:

- [x] Scope and owners mapped.
- [x] Existing contracts mapped to concrete source paths.
- [x] Integration order defined.
- [x] Theme mismatch documented.
- [x] Per-branch verification minimum defined.
- [ ] Teammate branches integrated.
- [ ] P0 theme presentation implemented.
- [ ] Full Play Mode journey verified.
- [ ] Reliability sprint committed/pushed.

No gameplay implementation or Play Mode verification was performed for this
review. No files outside this checklist were modified.
