using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    public string CurrentSceneName { get; private set; }
    public string PreviousSceneName { get; private set; }

    void Awake()
    {
        // enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // set starting scene
        CurrentSceneName = SceneManager.GetActiveScene().name;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PreviousSceneName = CurrentSceneName;
        CurrentSceneName = scene.name;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (!string.IsNullOrEmpty(PreviousSceneName))
        {
            SceneManager.LoadScene(PreviousSceneName);
        }
        else
        {
            Debug.LogWarning("No previous scene recorded!");
        }
    }
}
