using System;

namespace ELDIA.Gameplay.Shared
{
    public readonly struct ObjectiveCompletedEvent
    {
        public string ObjectiveId { get; }

        public ObjectiveCompletedEvent(string objectiveId)
        {
            ObjectiveId = objectiveId;
        }
    }

    /// <summary>
    /// Mission objective contract. TryComplete is idempotent: only the first
    /// call returns true and emits Completed.
    /// </summary>
    public interface IObjective
    {
        string ObjectiveId { get; }
        bool IsCompleted { get; }

        event Action<ObjectiveCompletedEvent> Completed;

        bool TryComplete();
    }
}
