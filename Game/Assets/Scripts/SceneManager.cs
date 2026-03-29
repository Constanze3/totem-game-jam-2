using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public List<string> preloadedScenes;
    public int initialOffset;
    public int offsetStep;

    [Header("Private properties, exposed for debugging")]
    public bool loading = false;
    public bool loaded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        PreloadScenes();
    }

    private void PreloadScenes()
    {
        if (loading || loaded)
        {
            return;
        }
        loading = true;

        StartCoroutine(PreloadScenesCoroutine());
    }

    private IEnumerator PreloadScenesCoroutine()
    {
        var offset = initialOffset;
        foreach (var sceneName in preloadedScenes)
        {
            var loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                sceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Additive
            );

            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }

            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                root.transform.position += new Vector3(0, offset, 0);
            }
            offset += offsetStep;
        }

        loading = false;
        loaded = true;
    }

    public UnityEngine.SceneManagement.Scene GetSceneByName(string name)
    {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
    }
}
