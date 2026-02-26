using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Optional: index or name of your main gameplay scene
    [Header("Scenes")]
    public int gameSceneBuildIndex = 1;

    public GameObject levelSelectScreen;
    public GameObject LogoScreen;

    public void Start()
    {
        levelSelectScreen.SetActive(false);
        LogoScreen.SetActive(true);
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneBuildIndex);
    }

    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Makes the Quit button work in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void LevelSelect()
    {
        levelSelectScreen.SetActive(true);
        LogoScreen.SetActive(false);
    }
}
