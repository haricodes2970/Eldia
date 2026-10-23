using System;

namespace ELDIA.Gameplay.Shared
{
    public enum GameState
    {
        Intro,
        Playing,
        PlayerDefeated,
        Victory
    }

    public readonly struct GameStateChangedEvent
    {
        public GameState PreviousState { get; }
        public GameState CurrentState { get; }

        public GameStateChangedEvent(GameState previousState, GameState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }

    /// <summary>
    /// Central state coordinator contract. Valid transitions:
    /// Intro -> Playing; Playing -> PlayerDefeated; Playing -> Victory.
    /// Restart policy is owned by the coordinator, not this shared contract.
    /// Victory is terminal for ordinary transitions; restarting creates a new
    /// run through coordinator policy rather than silently undoing victory.
    /// </summary>
    public static class GameStateRules
    {
        public static bool IsValidTransition(GameState current, GameState next)
        {
            if (current == next)
            {
                return false;
            }

            switch (current)
            {
                case GameState.Intro:
                    return next == GameState.Playing;
                case GameState.Playing:
                    return next == GameState.PlayerDefeated || next == GameState.Victory;
                case GameState.PlayerDefeated:
                case GameState.Victory:
                    return false;
                default:
                    return false;
            }
        }
    }

    public interface IGameState
    {
        GameState CurrentState { get; }

        event Action<GameStateChangedEvent> StateChanged;

        /// <summary>
        /// Implementations must call GameStateRules.IsValidTransition. Returns
        /// false for duplicate, invalid, or terminal-state transitions and
        /// must not emit StateChanged in those cases.
        /// </summary>
        bool TrySetState(GameState nextState);
    }

    /// <summary>
    /// Requests a restart without prescribing scene loading. The coordinator
    /// decides when and how a request becomes a new run.
    /// </summary>
    public interface IRestartRequester
    {
        event Action RestartRequested;

        void RequestRestart();
    }

    public readonly struct VictoryEvent
    {
        public string MissionId { get; }

        public VictoryEvent(string missionId)
        {
            MissionId = missionId;
        }
    }

    /// <summary>
    /// Mission victory notification. Implementations must require both
    /// objective completion and boss defeat before emitting VictoryTriggered.
    /// TryTriggerVictory is idempotent.
    /// </summary>
    public interface IVictoryState
    {
        bool HasWon { get; }

        event Action<VictoryEvent> VictoryTriggered;

        /// <summary>
        /// Returns false and emits nothing unless both prerequisites are true
        /// and victory has not already been triggered.
        /// </summary>
        bool TryTriggerVictory(bool objectiveCompleted, bool bossDefeated);
    }
}
