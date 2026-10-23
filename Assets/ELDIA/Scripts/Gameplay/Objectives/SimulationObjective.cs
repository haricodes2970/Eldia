using System;
using UnityEngine;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Objectives
{
    /// <summary>
    /// Mission objective representing a simulation control node.
    /// TryComplete is idempotent and changes the node to its disabled visual state once.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SimulationObjective : MonoBehaviour, IObjective
    {
        [SerializeField] private string objectiveId = "simulation-control-node";
        [SerializeField] private Color activeColor = new Color(0.1f, 0.85f, 1f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.2f, 0.25f, 0.3f, 1f);
        [SerializeField, Min(0.1f)] private float disabledScale = 0.8f;
        [SerializeField] private string activeStateLabel = "INTRUSION TARGET";
        [SerializeField] private string completedStateLabel = "SIMULATION NODE BREACHED";

        private SpriteRenderer nodeRenderer;
        private Vector3 activeScale;
        private bool isCompleted;

        public string ObjectiveId => objectiveId;
        public bool IsCompleted => isCompleted;
        public string StateLabel => isCompleted ? completedStateLabel : activeStateLabel;

        public event Action<ObjectiveCompletedEvent> Completed;

        private void Awake()
        {
            nodeRenderer = GetComponent<SpriteRenderer>();
            activeScale = transform.localScale;
            ApplyActiveVisual();
        }

        public bool TryComplete()
        {
            if (isCompleted)
            {
                return false;
            }

            isCompleted = true;
            ApplyDisabledVisual();
            Completed?.Invoke(new ObjectiveCompletedEvent(ObjectiveId));
            return true;
        }

        private void ApplyActiveVisual()
        {
            nodeRenderer.color = activeColor;
            transform.localScale = activeScale;
        }

        private void ApplyDisabledVisual()
        {
            nodeRenderer.color = disabledColor;
            transform.localScale = activeScale * disabledScale;
        }
    }
}
