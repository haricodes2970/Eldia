using UnityEngine;
using UnityEngine.SceneManagement;
using ELDIA.Gameplay.Combat;
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
        private PlayerGun gun;

        private void Awake() => gun = FindFirstObjectByType<PlayerGun>();

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

            if (gun != null) { GUI.color = gun.CurrentAmmo > 0 ? Color.yellow : Color.red; GUI.Label(new Rect(Screen.width - 180f, Screen.height - 52f, 160f, 32f), $"AMMO {gun.CurrentAmmo}/{gun.MaxAmmo}"); }
            if (showVictory && objective.IsCompleted && boss.IsDefeated)
            {
                GUI.color = new Color(0.3f, 1f, 0.75f, 1f);
                GUI.Box(new Rect(Screen.width * 0.5f - 220f, 70f, 440f, 90f), "DISTRICT SECURED\nPress R to run test again");
            }
            if (mission.CurrentState == GameState.Victory) { GUI.color = new Color(0f, 0f, 0f, 0.75f); GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), GUIContent.none); GUI.color = new Color(0f, 1f, 0.8f); GUI.Label(new Rect(Screen.width * .5f - 300f, Screen.height * .5f - 80f, 600f, 70f), "SIMULATION BREACHED"); GUI.color = Color.white; GUI.Label(new Rect(Screen.width * .5f - 300f, Screen.height * .5f, 600f, 35f), "You have earned a second life."); GUI.color = Color.gray; GUI.Label(new Rect(Screen.width * .5f - 300f, Screen.height * .5f + 35f, 600f, 30f), "Press ENTER to return to the main menu."); if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) SceneManager.LoadScene(0); }
        }
    }
}
