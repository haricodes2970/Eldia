# ELDIA Mission Coordinator

`MissionCoordinator` connects the existing simulation objective and District
Warden boss to the shared game-state and victory contracts.

Namespace:

```csharp
ELDIA.Gameplay.Mission
```

Interfaces:

```csharp
IGameState
IVictoryState
```

## Mission flow

At `Start`, coordinator requests:

```text
Intro -> Playing
```

This is accepted through `GameStateRules` and emits one
`GameStateChangedEvent` when valid.

Victory requires both live contract states:

```text
IObjective.IsCompleted == true
IBoss.IsDefeated == true
```

The coordinator then requests:

```text
Playing -> Victory
```

Only after that valid transition does it emit one `VictoryTriggered` event.

## Event subscriptions

The coordinator subscribes to:

- `IObjective.Completed`
- `IBoss.Defeated`

Subscriptions are guarded against duplicates and removed in `OnDisable`.
Each event refreshes completion from the actual `IObjective` and `IBoss` objects;
event payloads and caller-provided flags are not treated as authoritative state.

## Idempotence and safety

- Repeated objective events cannot trigger victory twice.
- Repeated boss events cannot trigger victory twice.
- `TryTriggerVictory(bool, bool)` ignores its Boolean arguments and derives
  state from the configured contracts.
- Victory cannot trigger unless current state is `Playing`.
- Invalid state transitions are rejected by `GameStateRules`.
- No scene loading, restart policy, menus, audio, or save systems are included.

## PlayerTest integration

`PlayerTestArena` creates `Mission Coordinator` after the objective and boss,
then calls:

```csharp
coordinator.Configure(objective, boss);
```

The existing player, ordinary enemy, objective, boss, movement, camera, health,
and combat objects remain in the arena.
