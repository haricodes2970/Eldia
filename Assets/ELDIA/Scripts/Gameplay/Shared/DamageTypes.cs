using System;
using UnityEngine;

namespace ELDIA.Gameplay.Shared
{
    /// <summary>
    /// Request passed to an IDamageable. Amount is requested damage; the target
    /// decides how much can actually be removed from health.
    /// </summary>
    [Serializable]
    public readonly struct DamageRequest
    {
        public int Amount { get; }
        public GameObject Source { get; }
        public Vector2 HitPoint { get; }
        public Vector2 Direction { get; }

        public DamageRequest(
            int amount,
            GameObject source = null,
            Vector2 hitPoint = default,
            Vector2 direction = default)
        {
            Amount = amount;
            Source = source;
            HitPoint = hitPoint;
            Direction = direction;
        }
    }

    /// <summary>
    /// Result returned by IDamageable.ReceiveDamage. AmountApplied is actual
    /// health lost, never merely the requested amount.
    /// </summary>
    [Serializable]
    public readonly struct DamageResult
    {
        public bool WasApplied { get; }
        public int AmountApplied { get; }
        public bool TargetDefeated { get; }

        public DamageResult(bool wasApplied, int amountApplied, bool targetDefeated)
        {
            WasApplied = wasApplied;
            AmountApplied = amountApplied;
            TargetDefeated = targetDefeated;
        }

        public static DamageResult NotApplied => new DamageResult(false, 0, false);
    }
}
