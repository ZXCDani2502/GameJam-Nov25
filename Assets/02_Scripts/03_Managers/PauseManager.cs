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

    public bool isPaused = false; // öffentlich, damit andere Scripts es problemlos lesen können

    [Header("Player Reference")]
    [Tooltip("Referenz auf den PlayerController")]
    public PlayerController player;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Sicherheit: Cursor standardmäßig fürs Gameplay sperren
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

        // Kamera deaktivieren
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

        // Kamera wieder aktivieren
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