# ELDIA Combat System — Integration Guide

## Overview

The combat system provides a minimal, modular foundation for player attacks and enemy damage reception. It builds on the shared contracts from Phase 4 and the HealthComponent from Phase 5.

Namespace: `ELDIA.Gameplay.Combat`

## Components

### PlayerAttack
Attaches to the player GameObject. Handles attack input and hit detection.

**Configuration (Inspector):**
| Field | Default | Description |
|-------|---------|-------------|
| `attackDamage` | 25 | Damage dealt per hit |
| `attackRange` | 1.5f | Size of the attack overlap box |
| `attackInputName` | "Fire1" | Legacy Input axis/button name (maps to Left Ctrl / J) |
| `targetLayers` | Everything | Layer mask for valid targets |
| `attackCooldown` | 0.3f | Minimum time between attacks |

**Behavior:**
- Reads attack input via `Input.GetButtonDown("Fire1")` (J key / Left Ctrl)
- Performs a `Physics2D.OverlapBoxAll` in the player's facing direction
- Applies damage through `IDamageable.ReceiveDamage(DamageRequest)`
- Prevents duplicate hits on the same target per attack via HashSet
- Excludes the player GameObject from targets
- Shows a brief visual hit effect at impact point

**Dependencies:**
- Requires `PlayerController2D` on same GameObject (for `FacingDirection`)
- Requires `Collider2D` on player (enforced by `RequireComponent`)

### BasicEnemy
Simple stationary enemy for combat testing.

**Configuration (Inspector):**
| Field | Default | Description |
|-------|---------|-------------|
| `healthComponent` | Auto | Reference to HealthComponent (auto-found) |
| `aliveColor` | Red | Color when alive |
| `defeatedColor` | Dark red, 50% alpha | Color when defeated |
| `defeatedFadeTime` | 1f | Time before enemy deactivates |

**Behavior:**
- Auto-adds/finds `HealthComponent` on Awake
- Subscribes to `HealthComponent.Defeated` event
- On defeat: fades out over `defeatedFadeTime`, then deactivates
- Implements `IDamageable` by forwarding to `HealthComponent`

## Integration in PlayerTestArena

The `PlayerTestArena` automatically creates:
1. **Player** at (-5, 0) with:
   - `PlayerController2D` (movement)
   - `HealthComponent` (100 HP)
   - `PlayerAttack` (25 damage, 1.5 range, J/Fire1 to attack)
   - Visual facing indicator

2. **Enemy** at (2, 0) with:
   - `HealthComponent` (50 HP)
   - `BasicEnemy` (defeat handling)
   - `BoxCollider2D`

3. **Arena walls** with collision
4. **Camera** following player

## Manual Test Steps in Unity

1. Open `Assets/ELDIA/Scenes/PlayerTest.unity`
2. Enter Play Mode
3. Move with WASD / Arrow keys
4. Face the enemy (movement updates facing direction)
5. Press **J** (or Left Ctrl) to attack
6. Observe:
   - Hit effect appears on successful hit
   - Enemy health decreases (not visible without HUD, but logic works)
   - After 2 hits (50 HP / 25 damage), enemy fades and deactivates
   - Player cannot damage itself
   - Attack cooldown prevents spam

## Key Design Decisions

- **No direct player reference in enemy**: Enemy doesn't know about player
- **Damage via contracts**: PlayerAttack creates `DamageRequest` with source = player GameObject
- **Duplicate hit prevention**: HashSet per attack frame
- **Self-damage prevention**: Explicit check `hit.gameObject == gameObject`
- **Legacy Input**: Uses `Input.GetButtonDown("Fire1")` - maps to J and Left Ctrl by default
- **Visual feedback**: Simple yellow flash effect (no particles package needed)

## Extending for Chetan (Combat Systems)

To add new attack types or enemy behaviors:

```csharp
// New weapon/attack pattern
public class SpecialAttack : MonoBehaviour
{
    public void Execute(Vector2 direction, int damage)
    {
        var request = new DamageRequest(damage, gameObject, position, direction);
        // ... query targets and apply
    }
}

// New enemy type
public class PatrolEnemy : MonoBehaviour, IDamageable
{
    // Implement movement + IDamageable
}
```

Both can use the existing `HealthComponent` and `IDamageable` contract.

## Notes for Divish (HUD)

Subscribe to health events on player/enemy:

```csharp
var playerHealth = player.GetComponent<HealthComponent>();
playerHealth.HealthChanged += OnPlayerHealthChanged;
playerHealth.Defeated += OnPlayerDefeated;

var enemyHealth = enemy.GetComponent<HealthComponent>();
enemyHealth.HealthChanged += OnEnemyHealthChanged;
enemyHealth.Defeated += OnEnemyDefeated;
```