using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Das Pause-Menü UI (Canvas oder Panel)")]
    public GameObject pauseMenuUI;

    [Header("Settings")]
    [Tooltip("Taste zum Öffnen/Schließen des Pause-Menüs")]
    public KeyCode pauseKey = KeyCode.Escape;

    public bool isPaused = false; // Public so other scripts can read it

    [Header("Player Reference")]
    [Tooltip("Referenz auf den PlayerController")]
    public PlayerController player;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // cursor lock as base that's lifted when playing (doesn't work the other way around idk why)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // deactivate camera movement
        if (player != null)
            player.SetLookState(false);

        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // reactivate camera movement
        if (player != null)
            player.SetLookState(true);

        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}