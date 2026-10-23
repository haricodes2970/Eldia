using UnityEngine;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Objectives
{
    /// <summary>
    /// Reads the legacy keyboard input and completes a nearby objective.
    /// The interaction key is E; no Input Manager asset changes are required.
    /// </summary>
    public sealed class ObjectiveInteractor : MonoBehaviour
    {
        [SerializeField] private SimulationObjective objective;
        [SerializeField, Min(0.1f)] private float interactionRange = 1.5f;
        [SerializeField] private string interactionPrompt = "Press E to breach terminal";

        public string InteractionPrompt => interactionPrompt;
        public bool IsInteractionAvailable => objective != null &&
            !objective.IsCompleted &&
            Vector2.Distance(transform.position, objective.transform.position) <= interactionRange;

        public void Configure(SimulationObjective configuredObjective)
        {
            objective = configuredObjective;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E) || objective == null)
            {
                return;
            }

            if (Vector2.Distance(transform.position, objective.transform.position) > interactionRange)
            {
                return;
            }

            IObjective objectiveContract = objective;
            objectiveContract.TryComplete();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
