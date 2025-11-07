using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleTextTMP;
    public GameObject optionsPanel;

    [Header("Story UI")]
    public GameObject storyPanel;
    public TMP_Text storyText;
    [TextArea] public string storyContent;

    [Header("Options UI Elements")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    [Header("Fade In")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 1.5f;

    private Resolution[] resolutions;

    // Fade panel is set up BEFORE Start() to prevent the transparency jump
    void Awake()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.alpha = 1f;              // Ensures it's fully black before rendering
            fadePanel.blocksRaycasts = false;  // Avoid UI conflicts
            fadePanel.interactable = false;
        }
    }

    void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (storyPanel != null)
            storyPanel.SetActive(false);

        // Start smooth fade
        if (fadePanel != null)
            StartCoroutine(FadeIn());

        // --- Options Setup ---
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
        if (titleTextTMP != null) titleTextTMP.gameObject.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (storyPanel != null)
        {
            storyPanel.SetActive(true);

            if (storyText != null)
                storyText.text = storyContent;
        }

        StartCoroutine(LoadGameAfterDelay(22.5f));
    }

    private IEnumerator LoadGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
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

    // --- Smooth Fade In Coroutine ---
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadePanel.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
    }
}