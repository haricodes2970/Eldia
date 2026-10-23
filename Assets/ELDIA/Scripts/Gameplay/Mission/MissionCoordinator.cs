using System;
using UnityEngine;
using ELDIA.Gameplay.Boss;
using ELDIA.Gameplay.Objectives;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Mission
{
    /// <summary>
    /// Coordinates the simulation objective, District Warden boss, game state,
    /// and mission victory. It does not load scenes or implement restart policy.
    /// </summary>
    public sealed class MissionCoordinator : MonoBehaviour, IGameState, IVictoryState
    {
        [SerializeField] private string missionId = "eldia-district-break";

        private IObjective objective;
        private IBoss boss;
        private IHealth playerHealth;
        private GameState currentState = GameState.Intro;
        private bool hasWon;
        private bool objectiveCompleted;
        private bool bossDefeated;
        private bool eventsSubscribed;

        public GameState CurrentState => currentState;
        public bool HasWon => hasWon;
        public bool ObjectiveCompleted => objectiveCompleted;
        public bool BossDefeated => bossDefeated;

        public event Action<GameStateChangedEvent> StateChanged;
        public event Action<VictoryEvent> VictoryTriggered;

        /// <summary>
        /// Wires concrete scene components to their shared contracts. The arena
        /// calls this after creating the objective and boss objects.
        /// </summary>
        public void Configure(IObjective missionObjective, IBoss missionBoss)
        {
            Configure(missionObjective, missionBoss, null);
        }

        public void Configure(IObjective missionObjective, IBoss missionBoss, IHealth missionPlayerHealth)
        {
            UnsubscribeFromEvents();
            objective = missionObjective;
            boss = missionBoss;
            playerHealth = missionPlayerHealth;
            RefreshCompletionState();
            SubscribeToEvents();
        }

        private void Start()
        {
            TrySetState(GameState.Playing);
            EvaluateVictory();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        public bool TrySetState(GameState nextState)
        {
            if (!GameStateRules.IsValidTransition(currentState, nextState))
            {
                return false;
            }

            if (nextState == GameState.Victory && !AreVictoryConditionsMet())
            {
                return false;
            }

            GameState previousState = currentState;
            currentState = nextState;
            StateChanged?.Invoke(new GameStateChangedEvent(previousState, currentState));
            return true;
        }

        /// <summary>
        /// Implements IVictoryState. Parameters are intentionally ignored as
        /// authority; live objective and boss contract state is always checked.
        /// </summary>
        public bool TryTriggerVictory(bool objectiveCompleted, bool bossDefeated)
        {
            RefreshCompletionState();
            return EvaluateVictory();
        }

        private bool EvaluateVictory()
        {
            if (hasWon || currentState != GameState.Playing || !AreVictoryConditionsMet())
            {
                return false;
            }

            if (!TrySetState(GameState.Victory))
            {
                return false;
            }

            hasWon = true;
            VictoryTriggered?.Invoke(new VictoryEvent(missionId));
            return true;
        }

        private bool AreVictoryConditionsMet()
        {
            return objective != null && boss != null && objective.IsCompleted && boss.IsDefeated;
        }

        private void RefreshCompletionState()
        {
            objectiveCompleted = objective != null && objective.IsCompleted;
            bossDefeated = boss != null && boss.IsDefeated;
        }

        private void SubscribeToEvents()
        {
            if (eventsSubscribed || objective == null || boss == null)
            {
                return;
            }

            objective.Completed += OnObjectiveCompleted;
            boss.Defeated += OnBossDefeated;
            if (playerHealth != null)
            {
                playerHealth.Defeated += OnPlayerDefeated;
            }

            eventsSubscribed = true;
        }

        private void UnsubscribeFromEvents()
        {
            if (!eventsSubscribed)
            {
                return;
            }

            if (objective != null)
            {
                objective.Completed -= OnObjectiveCompleted;
            }

            if (boss != null)
            {
                boss.Defeated -= OnBossDefeated;
            }

            if (playerHealth != null)
            {
                playerHealth.Defeated -= OnPlayerDefeated;
            }

            eventsSubscribed = false;
        }

        private void OnObjectiveCompleted(ObjectiveCompletedEvent payload)
        {
            RefreshCompletionState();
            EvaluateVictory();
        }

        private void OnBossDefeated(BossDefeatedEvent payload)
        {
            RefreshCompletionState();
            EvaluateVictory();
        }

        private void OnPlayerDefeated(EntityDefeatedEvent payload)
        {
            TrySetState(GameState.PlayerDefeated);
        }
    }
}
