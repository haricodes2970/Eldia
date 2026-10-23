using System;
using UnityEngine;

namespace ELDIA.Gameplay.Shared
{
    public readonly struct HealthChangedEvent
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public int Delta { get; }

        public HealthChangedEvent(int currentHealth, int maxHealth, int delta)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            Delta = delta;
        }
    }

    /// <summary>
    /// General entity defeat notification. Used by any health-bearing entity.
    /// It is not a boss lifecycle event.
    /// </summary>
    public readonly struct EntityDefeatedEvent
    {
        public GameObject Entity { get; }
        public GameObject Source { get; }

        public EntityDefeatedEvent(GameObject entity, GameObject source)
        {
            Entity = entity;
            Source = source;
        }
    }

    /// <summary>
    /// Observes health state. Implementations own health mutation and must
    /// raise Defeated once when health reaches zero.
    /// </summary>
    public interface IHealth
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsDefeated { get; }

        event Action<HealthChangedEvent> HealthChanged;
        event Action<EntityDefeatedEvent> Defeated;
    }

    /// <summary>
    /// Accepts damage. Implementations validate request.Amount at application
    /// time and return actual health lost through DamageResult.AmountApplied.
    /// </summary>
    public interface IDamageable
    {
        bool CanReceiveDamage { get; }

        DamageResult ReceiveDamage(DamageRequest request);
    }
}
