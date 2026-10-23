using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ELDIA.UI
{
    [RequireComponent(typeof(Camera))]
    public sealed class MainMenuController : MonoBehaviour
    {
        private GUIStyle title;
        private GUIStyle subtitle;
        private GUIStyle button;
        private readonly Color accent = new Color(0f, 1f, 0.8f);

        private void Awake()
        {
            Camera camera = GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.039f, 0.102f, 0.102f, 1f);
            camera.orthographic = true;
        }

        private void OnGUI()
        {
            BuildStyles();
            GUI.Label(new Rect(0f, Screen.height * 0.16f, Screen.width, 110f), "ELDIA", title);
            GUI.Label(new Rect(0f, Screen.height * 0.16f + 108f, Screen.width, 45f), "Break the Simulation", subtitle);

            float width = 320f;
            float height = 64f;
            float x = (Screen.width - width) * 0.5f;
            float y = Screen.height * 0.5f - 60f;
            if (PlayerPrefs.GetInt("ELDIA_SaveExists") == 1)
            {
                if (GUI.Button(new Rect(x, y, width, height), "CONTINUE", button)) LoadPlayerTest(false);
                y += height + 16f;
            }
            if (GUI.Button(new Rect(x, y, width, height), "NEW PLAYER", button)) LoadPlayerTest(true);
            y += height + 16f;
            if (GUI.Button(new Rect(x, y, width, height), "QUIT", button)) Quit();
        }

        private void BuildStyles()
        {
            title = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 96, fontStyle = FontStyle.Bold };
            title.normal.textColor = accent;
            subtitle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 28 };
            subtitle.normal.textColor = Color.white;
            button = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fontSize = 24, fontStyle = FontStyle.Bold };
            button.normal.textColor = accent;
            button.hover.textColor = Color.white;
        }

        private static void LoadPlayerTest(bool save)
        {
            PlayerPrefs.SetInt("ELDIA_SaveExists", 1);
            if (save) PlayerPrefs.Save();
            SceneManager.LoadScene(1);
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
