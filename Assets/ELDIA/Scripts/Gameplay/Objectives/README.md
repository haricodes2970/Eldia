# ELDIA Simulation Objective

## Components

### `SimulationObjective`

Namespace: `ELDIA.Gameplay.Objectives`

Implements the existing:

```csharp
ELDIA.Gameplay.Shared.IObjective
```

Default objective ID:

```text
simulation-control-node
```

`TryComplete()` is idempotent:

- First call returns `true`.
- First call changes the node from active cyan to disabled gray and scales it down.
- First call emits exactly one `ObjectiveCompletedEvent`.
- Later calls return `false` and emit no additional event.

The component does not load scenes, trigger victory, or mutate global game state.

### `ObjectiveInteractor`

The player receives this component at runtime in `PlayerTestArena`.

- Interaction key: **E** (`KeyCode.E`, legacy keyboard input)
- Default interaction range: `1.5` world units
- Interaction succeeds only when player distance to the objective is within range.
- Interaction is separate from `PlayerAttack` and does not use combat queries.
- Pressing E from outside range has no effect.

## PlayerTest integration

`PlayerTestArena` creates:

- `Simulation Control Node` at `(0, 3)`.
- `SimulationObjective` with the default stable objective ID.
- `ObjectiveInteractor` on the generated Player object, wired to the node.

Existing movement, camera, health, enemy, and attack setup remains active.
The arena does not create a boss, victory coordinator, scene loader, or UI.

## Manual test

1. Open `Assets/ELDIA/Scenes/PlayerTest.unity`.
2. Enter Play Mode.
3. Move with WASD or arrow keys.
4. Press **E** while far from the cyan node. It must remain active.
5. Move within 1.5 units of the node.
6. Press **E**. Node should become smaller and gray.
7. Press **E** repeatedly. No duplicate completion behavior should occur.
8. Subscribe to `IObjective.Completed` from a test listener to inspect:
   - `ObjectiveCompletedEvent.ObjectiveId == "simulation-control-node"`.
   - Exactly one event on repeated interaction.

## Event usage

```csharp
IObjective objective = node.GetComponent<SimulationObjective>();
objective.Completed += OnObjectiveCompleted;

private void OnObjectiveCompleted(ObjectiveCompletedEvent payload)
{
    // Mission systems may observe completion here.
    // This component itself does not trigger victory or scene loading.
}
```
