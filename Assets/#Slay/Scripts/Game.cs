using UnityEngine;
using UnityEngine.SceneManagement;

namespace Slay
{
    public class Game : MonoBehaviour
    {

        public bool escIsQuit = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (escIsQuit)
                    Application.Quit();
                else
                    SceneManager.LoadSceneAsync(0);
            }
        }
    }
}
