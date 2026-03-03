using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Optional: index or name of your main gameplay scene
    [Header("Scenes")]
    public int gameSceneBuildIndex = 1;

    public GameObject levelSelectScreen;
    public GameObject LogoScreen;
    public GameObject loadingScreen;

    public void Start()
    {
        levelSelectScreen.SetActive(false);
        LogoScreen.SetActive(true);
        loadingScreen.SetActive(false);
    }


    public void PlayGame()
    {
        //SceneManager.LoadScene(gameSceneBuildIndex);
        loadingScreen.SetActive(true );
        StartCoroutine(LoadYourAsyncScene());
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

    IEnumerator LoadYourAsyncScene()
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CityTest");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            //SceneManager.LoadScene("CityTest");
            yield return null;
        }
    }
}
