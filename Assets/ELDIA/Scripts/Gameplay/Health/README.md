# ELDIA Health Component — Integration Guide for Combat Systems

## Overview

`HealthComponent` implements both `IHealth` and `IDamageable` from `ELDIA.Gameplay.Shared`.
It is the standard health implementation for enemies, player, and destructible objects.

Namespace: `ELDIA.Gameplay.Health`

## Usage

### Adding to an entity

```csharp
using ELDIA.Gameplay.Health;

var health = gameObject.AddComponent<HealthComponent>();
health.maxHealth = 50;  // or set via Inspector
```

### Dealing damage (Chetan's combat code)

```csharp
using ELDIA.Gameplay.Shared;

public void Attack(IDamageable target, int damage, GameObject source, Vector2 hitPoint)
{
    var request = new DamageRequest(damage, source, hitPoint);
    var result = target.ReceiveDamage(request);

    if (result.WasApplied)
    {
        // Damage was applied: result.AmountApplied is actual health lost
        // result.TargetDefeated indicates if this hit defeated the target
    }
}
```

### Subscribing to health events (Divish's HUD code)

```csharp
using ELDIA.Gameplay.Shared;
using ELDIA.Gameplay.Health;

var health = target.GetComponent<HealthComponent>();

health.HealthChanged += OnHealthChanged;
health.Defeated += OnEntityDefeated;

void OnHealthChanged(HealthChangedEvent e)
{
    // e.CurrentHealth, e.MaxHealth, e.Delta (negative for damage, positive for heal)
}

void OnEntityDefeated(EntityDefeatedEvent e)
{
    // e.Entity is the defeated GameObject
    // e.Source is the damage source (may be null)
}
```

### Healing (if needed)

```csharp
// Returns actual amount healed (clamped to max health, 0 if defeated)
int healed = health.Heal(25);
```

## Behavior Summary

| Scenario | Behavior |
|----------|----------|
| Damage amount <= 0 | Ignored, returns `NotApplied` |
| Damage after defeat | Ignored, returns `NotApplied` |
| Damage exceeding current health | Applied as overkill (AmountApplied = remaining health) |
| Heal amount <= 0 | Ignored, returns 0 |
| Heal after defeat | Ignored, returns 0 |
| Heal beyond max health | Clamped to max |
| Defeat event | Fired exactly once when health reaches 0 |
| HealthChanged event | Fired only when health value actually changes |

## Configuration

| Field | Default | Description |
|-------|---------|-------------|
| `maxHealth` | 100 | Maximum health (min 1, enforced in Awake) |
| `currentHealth` | 100 | Starting health (clamped to [0, maxHealth]) |
| `canReceiveDamage` | true | Toggle damage reception at runtime |

## Notes for Chetan

- `ReceiveDamage` validates at application time per contract.
- `DamageResult.AmountApplied` is actual health lost, not requested damage.
- Use `DamageRequest.Source` to track kill attribution.
- The component requires a `Collider2D` (enforced by `RequireComponent`).
- For boss entities, implement `IBoss` separately or extend this component.

## Notes for Divish

- Subscribe in `OnEnable` / `OnDisable` or `Start` / `OnDestroy`.
- `HealthChangedEvent.Delta` is signed: negative for damage, positive for heal.
- `EntityDefeatedEvent.Source` may be null if damage source was unknown.