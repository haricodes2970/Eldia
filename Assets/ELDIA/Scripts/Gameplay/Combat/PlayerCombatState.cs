using UnityEngine;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Combat
{
    /// <summary>
    /// Bridges player health defeat into the existing mission state contract.
    /// It does not own health or mission transitions beyond one defeat request.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public sealed class PlayerCombatState : MonoBehaviour
    {
        private HealthComponent health;
        private PlayerController2D controller;
        private PlayerAttack attack;
        private IGameState gameState;
        private bool subscribed;
        private bool defeated;

        public void Configure(IGameState configuredGameState)
        {
            gameState = configuredGameState;
            ApplyGameplayLock();
        }

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            controller = GetComponent<PlayerController2D>();
            attack = GetComponent<PlayerAttack>();
        }

        private void OnEnable()
        {
            Subscribe();
            ApplyGameplayLock();
        }

        private void OnDisable()
        {
            if (subscribed && health != null)
            {
                health.Defeated -= OnDefeated;
                subscribed = false;
            }
        }

        private void Update()
        {
            ApplyGameplayLock();
        }

        private void Subscribe()
        {
            if (subscribed || health == null)
            {
                return;
            }

            health.Defeated += OnDefeated;
            subscribed = true;
        }

        private void OnDefeated(EntityDefeatedEvent payload)
        {
            if (defeated)
            {
                return;
            }

            defeated = true;
            if (gameState != null && gameState.CurrentState == GameState.Playing)
            {
                gameState.TrySetState(GameState.PlayerDefeated);
            }

            ApplyGameplayLock();
        }

        private void ApplyGameplayLock()
        {
            bool canAct = !defeated && (gameState == null || gameState.CurrentState == GameState.Playing);
            if (controller != null)
            {
                controller.SetGameplayEnabled(canAct);
            }

            if (attack != null)
            {
                attack.SetGameplayEnabled(canAct);
            }
        }
    }
}
