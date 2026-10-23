# ELDIA Basic Boss Foundation

## Component

`BasicBoss` implements both:

```csharp
ELDIA.Gameplay.Shared.IBoss
ELDIA.Gameplay.Shared.IDamageable
```

Namespace:

```csharp
ELDIA.Gameplay.Boss
```

## Configuration

| Field | Default | Purpose |
|---|---:|---|
| `bossId` | `district-warden` | Stable mission identifier |
| `configuredMaxHealth` | `150` | Used when the component creates its own HealthComponent |
| `aliveColor` | Purple | Active boss visual |
| `hitColor` | Gold | Brief damage feedback |
| `defeatedColor` | Dark purple | Defeated visual |
| `hitFeedbackDuration` | `0.12` seconds | Hit-color duration |

PlayerTest creates the boss with `HealthComponent` max health `150`, compared
with the ordinary enemy's `50` health.

## Contract behavior

- `BasicBoss.ReceiveDamage` forwards requests to the existing `HealthComponent`.
- It does not duplicate health, overkill, or damage validation logic.
- `PlayerAttack` finds the boss through `IDamageable`; no attack changes are needed.
- `BasicBoss` subscribes to `HealthComponent.HealthChanged` and `Defeated` in
  `OnEnable`, and unsubscribes in `OnDisable`.
- Damage briefly changes the boss visual to `hitColor`.
- Entity defeat changes the visual to `defeatedColor` and scales it down.
- `BossDefeatedEvent` is emitted once only using:

```csharp
new BossDefeatedEvent(BossId, gameObject)
```

- After defeat, `CanReceiveDamage` is false and `ReceiveDamage` returns
  `DamageResult.NotApplied`.
- This component does not load scenes or trigger victory.

## Manual test

1. Open `Assets/ELDIA/Scenes/PlayerTest.unity`.
2. Enter Play Mode.
3. Move toward the larger purple `District Warden Boss` at `(6, 3.5)`.
4. Face it and press J / the configured `Fire1` input.
5. Confirm hit feedback changes the boss briefly to gold.
6. Continue attacking until 6 hits of 25 damage defeat the 150-health boss.
7. Confirm it changes to dark purple and shrinks.
8. Further attacks should not apply damage or emit another boss defeat event.

Runtime verification is pending until Unity compilation and Play Mode are available.
