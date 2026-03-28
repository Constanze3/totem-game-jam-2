using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private int sceneNO = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNO, LoadSceneMode.Additive);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
