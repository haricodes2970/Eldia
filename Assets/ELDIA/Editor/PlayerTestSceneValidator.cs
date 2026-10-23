#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ELDIA.Editor
{
    public static class PlayerTestSceneValidator
    {
        private const string ScenePath = "Assets/ELDIA/Scenes/PlayerTest.unity";

        [MenuItem("ELDIA/Validate Player Test Scene")]
        public static void Validate()
        {
            if (!File.Exists(ScenePath))
            {
                throw new FileNotFoundException("PlayerTest scene missing", ScenePath);
            }

            if (!EditorBuildSettingsSceneIsEnabled(ScenePath))
            {
                throw new InvalidDataException("PlayerTest scene is not enabled in build settings.");
            }

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset == null)
            {
                throw new InvalidDataException("PlayerTest scene could not be imported.");
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            PlayerTestArena arena = Object.FindObjectOfType<PlayerTestArena>();
            if (arena == null)
            {
                throw new InvalidDataException("PlayerTestArena component missing from scene.");
            }

            if (arena.GetComponent<Transform>() == null)
            {
                throw new InvalidDataException("PlayerTestArena is missing its Transform component.");
            }

            string[] requiredScripts =
            {
                "Assets/ELDIA/Scripts/Player/PlayerController2D.cs",
                "Assets/ELDIA/Scripts/Player/CameraFollow2D.cs",
                "Assets/ELDIA/Scripts/Gameplay/Combat/PlayerAttack.cs",
                "Assets/ELDIA/Scripts/Gameplay/Health/HealthComponent.cs",
                "Assets/ELDIA/Scripts/Gameplay/Objectives/SimulationObjective.cs",
                "Assets/ELDIA/Scripts/Gameplay/Boss/BasicBoss.cs",
                "Assets/ELDIA/Scripts/Gameplay/Mission/MissionCoordinator.cs",
                "Assets/ELDIA/Scripts/Gameplay/Presentation/ArenaHUD.cs",
                "Assets/ELDIA/Scripts/Gameplay/Reset/TestArenaReset.cs"
            };

            foreach (string scriptPath in requiredScripts)
            {
                if (AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath) == null)
                {
                    throw new InvalidDataException("Required script missing or not imported: " + scriptPath);
                }
            }

            Debug.Log("ELDIA PlayerTest scene validation passed: " + scene.path);
        }

        private static bool EditorBuildSettingsSceneIsEnabled(string scenePath)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath && scene.enabled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
