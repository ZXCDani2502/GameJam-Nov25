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

    [Header("Play / Story Controls")]
    public Button skipButton;

    private Resolution[] resolutions;
    private Coroutine autoLoadRoutine;          // Reference to stop coroutine

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(false);

        // Hide skip button at start
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        // Fade in
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            StartCoroutine(FadeIn());
        }

        // Options setup (probably not working at all but who knows?)
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

    // MainMenu buttons
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

        // Show skip button
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipToGame);
        }

        // Start the timed auto-load
        autoLoadRoutine = StartCoroutine(LoadGameAfterDelay(45f));
    }

    private IEnumerator LoadGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene("Game_Scene");
    }

    // Called by Skip button
    public void SkipToGame()
    {
        // Stop auto timer if still running
        if (autoLoadRoutine != null)
            StopCoroutine(autoLoadRoutine);

        SceneManager.LoadScene("Game_Scene");
    }

    // Fade in coroutine
    private IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
    }

    // Options functions
    public void SetVolume(float volume) => AudioListener.volume = volume;

    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

    public void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }
}
