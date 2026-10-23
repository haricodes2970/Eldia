using System;
using UnityEngine;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Health
{
    /// <summary>
    /// Concrete health implementation for entities that can take damage and be defeated.
    /// Implements IHealth and IDamageable contracts. Healing a defeated entity is not allowed.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class HealthComponent : MonoBehaviour, IHealth, IDamageable
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;
        [SerializeField] private bool canReceiveDamage = true;

        private bool isDefeated;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDefeated => isDefeated;
        public bool CanReceiveDamage => canReceiveDamage && !isDefeated;

        public event Action<HealthChangedEvent> HealthChanged;
        public event Action<EntityDefeatedEvent> Defeated;

        private void Awake()
        {
            ValidateInitialHealth();
        }

        private void ValidateInitialHealth()
        {
            if (maxHealth < 1)
            {
                maxHealth = 1;
            }

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            isDefeated = currentHealth <= 0;
        }

        public DamageResult ReceiveDamage(DamageRequest request)
        {
            if (!CanReceiveDamage)
            {
                return DamageResult.NotApplied;
            }

            if (request.Amount <= 0)
            {
                return DamageResult.NotApplied;
            }

            int previousHealth = currentHealth;
            int actualDamage = Math.Min(request.Amount, currentHealth);
            currentHealth -= actualDamage;
            bool wasDefeatedThisHit = !isDefeated && currentHealth <= 0;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDefeated = true;
            }

            int delta = currentHealth - previousHealth;
            if (delta != 0)
            {
                HealthChanged?.Invoke(new HealthChangedEvent(currentHealth, maxHealth, delta));
            }

            if (wasDefeatedThisHit)
            {
                Defeated?.Invoke(new EntityDefeatedEvent(gameObject, request.Source));
            }

            return new DamageResult(true, actualDamage, wasDefeatedThisHit);
        }

        /// <summary>
        /// Heals the entity by the given amount. Does not heal defeated entities.
        /// Returns the actual amount healed.
        /// </summary>
        public int Heal(int amount)
        {
            if (isDefeated)
            {
                return 0;
            }

            if (amount <= 0)
            {
                return 0;
            }

            int previousHealth = currentHealth;
            int actualHeal = Math.Min(amount, maxHealth - currentHealth);
            currentHealth += actualHeal;

            int delta = currentHealth - previousHealth;
            if (delta != 0)
            {
                HealthChanged?.Invoke(new HealthChangedEvent(currentHealth, maxHealth, delta));
            }

            return actualHeal;
        }

        /// <summary>
        /// Sets max health and clamps current health to the new range.
        /// </summary>
        public void SetMaxHealth(int newMaxHealth)
        {
            if (newMaxHealth < 1)
            {
                newMaxHealth = 1;
            }

            maxHealth = newMaxHealth;

            if (currentHealth > maxHealth)
            {
                int previousHealth = currentHealth;
                currentHealth = maxHealth;
                int delta = currentHealth - previousHealth;
                if (delta != 0)
                {
                    HealthChanged?.Invoke(new HealthChangedEvent(currentHealth, maxHealth, delta));
                }
            }
        }

        /// <summary>
        /// Resets health to max and clears defeated state. Use for object pooling or respawning.
        /// </summary>
        public void ResetHealth()
        {
            int previousHealth = currentHealth;
            currentHealth = maxHealth;
            isDefeated = false;

            int delta = currentHealth - previousHealth;
            if (delta != 0)
            {
                HealthChanged?.Invoke(new HealthChangedEvent(currentHealth, maxHealth, delta));
            }
        }
    }
}