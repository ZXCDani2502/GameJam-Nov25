using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Needed for Coroutines

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleTextTMP;          // Updated to TMP_Text
    public GameObject optionsPanel;    

    [Header("Story UI")]
    public GameObject storyPanel;            // Panel for story cutscene
    public TMP_Text storyText;               // Text component inside the panel
    [TextArea] public string storyContent;   // Your story text

    [Header("Options UI Elements")]
    public Slider volumeSlider;         
    public Toggle fullscreenToggle;     
    public TMP_Dropdown resolutionDropdown; 

    private Resolution[] resolutions;

    void Start()
    {
        // Hide options at start
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // Hide story panel at start
        if (storyPanel != null)
            storyPanel.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            var options = new System.Collections.Generic.List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
    }

    // --- MAIN MENU BUTTONS ---
    public void PlayGame()
    {
        // Hide main menu title text
        if (titleTextTMP != null) titleTextTMP.gameObject.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Show story panel
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);

            if (storyText != null)
                storyText.text = storyContent; // Set your story text
        }

        // Start coroutine to load game scene after 10 seconds
        StartCoroutine(LoadGameAfterDelay(10f));
    }

    private IEnumerator LoadGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // uses real time, ignores Time.timeScale
        SceneManager.LoadScene("Game_Scene");
    }

    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (titleTextTMP != null) titleTextTMP.gameObject.SetActive(false);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (titleTextTMP != null) titleTextTMP.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }

    // --- OPTIONS FUNCTIONS ---
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}