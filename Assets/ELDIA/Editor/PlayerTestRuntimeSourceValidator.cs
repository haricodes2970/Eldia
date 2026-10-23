#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ELDIA.Editor
{
    /// <summary>
    /// Verifies source-level lifecycle invariants that can be checked without
    /// entering Play Mode or mutating the test scene.
    /// </summary>
    public static class PlayerTestRuntimeSourceValidator
    {
        private const string ArenaPath = "Assets/ELDIA/Scripts/Core/PlayerTestArena.cs";
        private const string PlayerPath = "Assets/ELDIA/Scripts/Player/PlayerController2D.cs";
        private const string CameraPath = "Assets/ELDIA/Scripts/Player/CameraFollow2D.cs";

        [MenuItem("ELDIA/Validate Runtime Ownership")]
        public static void Validate()
        {
            string arenaSource = ReadRequired(ArenaPath);
            string playerSource = ReadRequired(PlayerPath);
            string cameraSource = ReadRequired(CameraPath);

            Require(arenaSource, "private bool initialized;", "Arena must guard repeated initialization.");
            Require(arenaSource, "if (initialized)", "Arena must guard repeated initialization.");
            Require(arenaSource, "generatedRoot", "Arena must own generated objects.");
            Require(arenaSource, "cameraObject.transform.SetParent(generatedRoot, true)", "Generated camera must be arena-owned.");
            Require(arenaSource, "Destroy(existing.gameObject)", "Arena cleanup must destroy owned generated root.");
            Require(arenaSource, "controller.Configure(facing.transform)", "Player wiring must use explicit configuration.");
            Reject(arenaSource, "GameObject.Find(\"Main Camera\")", "Arena must not globally delete cameras.");
            Reject(arenaSource, "SetPrivateField", "Arena must not use reflection for project-owned wiring.");
            Require(playerSource, "public void Configure(Transform configuredFacingIndicator)", "Player must expose explicit indicator configuration.");
            Require(cameraSource, "public void Configure(Transform configuredTarget)", "Camera must expose explicit target configuration.");

            Debug.Log("ELDIA runtime ownership validation passed.");
        }

        private static string ReadRequired(string path)
        {
            string source = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new InvalidDataException("Required source is empty: " + path);
            }

            return source;
        }

        private static void Require(string source, string expected, string message)
        {
            if (!source.Contains(expected))
            {
                throw new InvalidDataException(message + " Missing: " + expected);
            }
        }

        private static void Reject(string source, string forbidden, string message)
        {
            if (source.Contains(forbidden))
            {
                throw new InvalidDataException(message + " Found: " + forbidden);
            }
        }
    }
}
#endif
