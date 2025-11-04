using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject titleText;      // Assign your TextMeshPro "Title" object here
    public GameObject optionsPanel;   // Assign your Options Panel here

    public void PlayGame()
    {
        SceneManager.LoadScene("Game_Scene");
    }

    public void OpenOptions()
    {
        // Show options panel
        optionsPanel.SetActive(true);

        // Hide the title text
        if (titleText != null)
            titleText.SetActive(false);
    }

    public void CloseOptions()
    {
        // Hide options panel
        optionsPanel.SetActive(false);

        // Show the title text again
        if (titleText != null)
            titleText.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }
}