using UnityEngine;
using UnityEngine.SceneManagement;

namespace Slay
{
    public class Game : MonoBehaviour
    {

        public bool backIsQuit = false;

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
            if (InputHandler.Instance.BackTriggered)
            {
                if (backIsQuit)
                    Application.Quit();
                else
                    SceneManager.LoadSceneAsync(0);
            }
        }
    }
}
