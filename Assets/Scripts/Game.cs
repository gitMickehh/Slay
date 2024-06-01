using UnityEngine;

namespace Slay
{
    public class Game : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Application.Quit();
            }
        }
    }
}
