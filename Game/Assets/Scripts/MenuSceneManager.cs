using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public class MenuSceneManager : MonoBehaviour
    {

        public void LoadNextScene()
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentIndex + 1);
        }

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }

    }
}
