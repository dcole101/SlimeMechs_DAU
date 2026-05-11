using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] PlayerInput playerInput;

    [Header("Settings")]
    public string playerActionMapName = "Player";
    public string uiActionMapName = "UI";
    public Key pauseKey = Key.Escape;
    public Key testkey = Key.F;
    public Key testkey2 = Key.G;

    bool isPaused;
    private const string MAIN_MENU_SCENE = "MainMenu";
    private const string VICTORY_DIALOGUE = "VictoryDialogue";


    [Header("Victory Screen")]
    [SerializeField] GameObject victoryCanvas; // Boss Defeated UI Canvas
    [SerializeField] RectTransform maskRect;
    [SerializeField] GameObject vicObject;

    [SerializeField] GameObject diecanvas; 
    [SerializeField] RectTransform maskRect2;

    void Awake()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[pauseKey].wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }

        //if (Keyboard.current != null && Keyboard.current[testkey].wasPressedThisFrame)
        //{
        //    ShowVictoryScreen();
        //}

        //if (Keyboard.current != null && Keyboard.current[testkey2].wasPressedThisFrame)
        //{
        //    ShowDieScreen();
        //}

    }

    void SwitchToUI()
    {
        if (playerInput == null || playerInput.actions == null) return;

        var uiMap = playerInput.actions.FindActionMap(uiActionMapName, throwIfNotFound: false);
        var playerMap = playerInput.actions.FindActionMap(playerActionMapName, throwIfNotFound: false);

        if (uiMap != null) uiMap.Enable();
        if (playerMap != null) playerMap.Disable();
    }

    void SwitchToPlayer()
    {
        if (playerInput == null || playerInput.actions == null) return;

        var playerMap = playerInput.actions.FindActionMap(playerActionMapName, throwIfNotFound: false);
        var uiMap = playerInput.actions.FindActionMap(uiActionMapName, throwIfNotFound: false);

        if (playerMap != null) playerMap.Enable();
        if (uiMap != null) uiMap.Disable();
    }

    public void Pause()
    {
        //if (isPaused) return;

        //isPaused = true;
        //Time.timeScale = 0f;
        //if (pauseMenuUI != null)
        //    pauseMenuUI.SetActive(true);

        //SwitchToUI();

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        SetPaused(true);
    }

    public void Resume()
    {
        //if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        victoryCanvas.SetActive(false);
        diecanvas.SetActive(false);

        SwitchToPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        

        //SetPaused(false);
    }

    void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(paused);

        if (paused) SwitchToUI();
        else SwitchToPlayer();

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SwitchToPlayer();

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SwitchToPlayer();

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(MAIN_MENU_SCENE);

    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SwitchToPlayer();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void VictoryDialogue()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SwitchToPlayer();

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(VICTORY_DIALOGUE);

    }

    public void ShowVictoryScreen()
    {
        SwitchToUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start wipe animation
        if (maskRect != null)
            StartCoroutine(WipeLeftToRight(victoryCanvas, maskRect));
    }

    public void ShowDieScreen()
    {
        //Time.timeScale = true ? 0f : 1f;
        SwitchToUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        vicObject.SetActive(false);

        // Start wipe animation
        if (maskRect2 != null)
            StartCoroutine(WipeLeftToRight(diecanvas, maskRect2));
    }

    private IEnumerator WipeLeftToRight(GameObject activecanvas, RectTransform rect)
    {
        activecanvas.SetActive(true);
        Debug.Log("Started Wipe Animation");

        Vector2 originalSize = rect.sizeDelta;
        rect.sizeDelta = new Vector2(0, originalSize.y);

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float width = Mathf.Lerp(0, 1920f, elapsed / duration);
            rect.sizeDelta = new Vector2(width, originalSize.y);
            yield return null;
        }

        rect.sizeDelta = originalSize;

        yield return new WaitForSeconds(2);
        Time.timeScale = true ? 0f : 1f;
    }
}
