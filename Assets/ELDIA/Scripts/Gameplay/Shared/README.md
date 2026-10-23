# ELDIA shared gameplay contracts

Namespace: `ELDIA.Gameplay.Shared`

These files define data and event contracts only. They contain no scene,
UI, combat, boss, objective-node, or scene-reload implementation.

## Damage and health

- `DamageRequest` carries positive/invalid requested amount, optional source,
  hit point, and direction.
- `IDamageable.ReceiveDamage` validates damage at application time.
  Non-positive requests return `DamageResult.NotApplied`.
- `DamageResult.AmountApplied` means actual health removed, not requested
  damage. A target with 2 health receiving 5 damage reports `2`.
- `IHealth` exposes health state and notifications. It does not accept damage.
- `IDamageable` accepts damage. A concrete health component may implement both.
- `EntityDefeatedEvent` is the general notification for any health-bearing
  entity. It is distinct from boss mission progression.

## Bosses

`IBoss.Defeated` emits `BossDefeatedEvent`, identified by `BossId`. A concrete
boss must guard publication so this event is emitted once only. Boss code may
also emit the general `IHealth.Defeated` event; consumers should choose the
specific event matching their responsibility.

## Objectives

`IObjective.TryComplete()` is idempotent. First completion returns `true` and
emits `Completed`; later calls return `false` and emit nothing. Objective IDs
are stable mission identifiers, not display text.

## Game state

Use `GameStateRules.IsValidTransition` before changing state:

```text
Intro -> Playing
Playing -> PlayerDefeated
Playing -> Victory
```

Duplicate transitions and transitions out of `PlayerDefeated` or `Victory` are
invalid. Invalid transitions return `false` and emit no notification. Victory
is not silently undone by ordinary state changes. Restart policy belongs to the
coordinator and may create a new run explicitly; `IRestartRequester` only
publishes a request and never reloads a scene.

## Victory

`IVictoryState.TryTriggerVictory(objectiveCompleted, bossDefeated)` returns
`true` and emits `VictoryTriggered` only when both flags are true and victory
has not already occurred. It returns `false` without emitting for missing
prerequisites or repeated calls. A mission coordinator should derive the two
flags from `IObjective.IsCompleted` and `IBoss.IsDefeated`.

## Ownership handoff

- **Hari**: shared contracts, state coordinator, restart policy, mission
  coordinator, player integration, and final scene wiring.
- **Chetan**: combat, attacks, health implementations, damage application,
  enemies, and boss implementation. Consume `DamageRequest`, implement
  `IDamageable`/`IHealth`/`IBoss` as appropriate.
- **Pannaga**: level/environment and simulation-node presentation. Implement
  `IObjective` for the simulation objective; publish completion without making
  UI or scene reload calls.
- **Divish**: HUD and presentation. Subscribe to `IHealth.HealthChanged`,
  `IHealth.Defeated`, `IObjective.Completed`, `IBoss.Defeated`, and
  `IVictoryState.VictoryTriggered`; do not own gameplay state or reload scenes.

Subscribe in lifecycle methods and unsubscribe symmetrically. Event payloads
are value types and contain no presentation assumptions.
