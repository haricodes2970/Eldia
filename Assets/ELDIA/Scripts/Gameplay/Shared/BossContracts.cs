using System;
using UnityEngine;

namespace ELDIA.Gameplay.Shared
{
    /// <summary>
    /// Boss-specific defeat notification. Separate from EntityDefeatedEvent so
    /// mission flow can observe boss completion without treating every defeated
    /// entity as the boss. A boss implementation must publish this once only.
    /// </summary>
    public readonly struct BossDefeatedEvent
    {
        public string BossId { get; }
        public GameObject Boss { get; }

        public BossDefeatedEvent(string bossId, GameObject boss)
        {
            BossId = bossId;
            Boss = boss;
        }
    }

    public interface IBoss
    {
        string BossId { get; }
        bool IsDefeated { get; }

        event Action<BossDefeatedEvent> Defeated;
    }
}
