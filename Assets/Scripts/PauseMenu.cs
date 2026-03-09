using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] PlayerInput playerInput;

    [Header("Settings")]
    public string playerActionMapName = "Player";
    public string uiActionMapName = "UI";
    public Key pauseKey = Key.Escape;

    bool isPaused;

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
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        SwitchToUI();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        SwitchToPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        SceneManager.LoadScene("MainMenu");
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
}
