using UnityEngine;
using UnityEngine.SceneManagement;

namespace ELDIA.Gameplay.Reset
{
    /// <summary>
    /// Local reset control for the PlayerTest scene. It deliberately does not
    /// change shared restart contracts or implement a global restart policy.
    /// </summary>
    public sealed class TestArenaReset : MonoBehaviour
    {
        [SerializeField] private KeyCode resetKey = KeyCode.R;

        private void Update()
        {
            if (Input.GetKeyDown(resetKey))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
