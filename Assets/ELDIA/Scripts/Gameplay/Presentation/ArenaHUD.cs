using UnityEngine;
using ELDIA.Gameplay.Boss;
using ELDIA.Gameplay.Objectives;
using ELDIA.Gameplay.Mission;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Presentation
{
    /// <summary>
    /// Small runtime HUD for the PlayerTest arena. It observes existing
    /// objective, boss, and mission contracts; it does not own gameplay state.
    /// Uses built-in IMGUI so no UI package is required.
    /// </summary>
    public sealed class ArenaHUD : MonoBehaviour
    {
        private IObjective objective;
        private IBoss boss;
        private MissionCoordinator mission;
        private bool subscribed;
        private bool showVictory;

        public void Configure(IObjective missionObjective, IBoss missionBoss, MissionCoordinator missionCoordinator)
        {
            Unsubscribe();
            objective = missionObjective;
            boss = missionBoss;
            mission = missionCoordinator;
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (subscribed || objective == null || boss == null || mission == null)
            {
                return;
            }

            objective.Completed += OnObjectiveCompleted;
            boss.Defeated += OnBossDefeated;
            mission.VictoryTriggered += OnVictory;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed)
            {
                return;
            }

            objective.Completed -= OnObjectiveCompleted;
            boss.Defeated -= OnBossDefeated;
            mission.VictoryTriggered -= OnVictory;
            subscribed = false;
        }

        private void OnObjectiveCompleted(ObjectiveCompletedEvent payload) => Refresh();
        private void OnBossDefeated(BossDefeatedEvent payload) => Refresh();

        private void OnVictory(VictoryEvent payload)
        {
            showVictory = true;
            Refresh();
        }

        private void Refresh()
        {
            if (mission != null && mission.HasWon)
            {
                showVictory = true;
            }
        }

        private void OnGUI()
        {
            if (objective == null || boss == null || mission == null)
            {
                return;
            }

            GUI.color = Color.white;
            GUI.Label(new Rect(24f, 20f, 420f, 28f), "ELDIA // DISTRICT TEST ARENA");
            GUI.Label(new Rect(24f, 48f, 520f, 46f), "Move: WASD / Arrows   Attack: Fire1 (Ctrl/J)\nInteract: E near node   Restart: R");
            GUI.Label(new Rect(24f, 104f, 360f, 72f), string.Format(
                "Objective: {0}\nDistrict Warden: {1}\nState: {2}",
                objective.IsCompleted ? "COMPLETE" : "INCOMPLETE",
                boss.IsDefeated ? "DEFEATED" : "ACTIVE",
                mission.CurrentState));

            if (showVictory && objective.IsCompleted && boss.IsDefeated)
            {
                GUI.color = new Color(0.3f, 1f, 0.75f, 1f);
                GUI.Box(new Rect(Screen.width * 0.5f - 220f, 70f, 440f, 90f), "DISTRICT SECURED\nPress R to run test again");
            }
        }
    }
}
