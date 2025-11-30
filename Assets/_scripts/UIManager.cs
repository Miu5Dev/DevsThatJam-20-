using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelMainButtons;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelCredits;
    [SerializeField] private GameObject panelConfirmQuit;

    [Header("Musica")]
    [SerializeField] private AudioClip menuMusic;
    private AudioSource audioSource;


    [Header("Main Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Options Buttons")]
    [SerializeField] private Button backFromOptionsButton;
    [SerializeField] private Button backFromCreditsButton;
    [SerializeField] private Button quitYesButton;
    [SerializeField] private Button quitNoButton;

    [Header("Opciones")]
    [SerializeField] private Slider sliderMasterVolume;
    [SerializeField] private Toggle toggleFullscreen;

    [Header("Datos de juego")]
    [SerializeField] private string gameplaySceneName = "GameScene";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            
        }
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 0f;

        if (playButton != null) playButton.onClick.AddListener(OnPlayButton);
        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptionsButton);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnCreditsButton);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitButton);

        if (backFromOptionsButton != null) backFromOptionsButton.onClick.AddListener(OnBackFromOptions);
        if (backFromCreditsButton != null) backFromCreditsButton.onClick.AddListener(OnBackFromCredits);
        if (quitYesButton != null) quitYesButton.onClick.AddListener(OnQuitYes);
        if (quitNoButton != null) quitNoButton.onClick.AddListener(OnQuitNo);
    }

    private void Start()
    {
        if (menuMusic != null && audioSource != null)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        
        ShowMainMenu();
        InitOptionsUI();
    }

    private void InitOptionsUI()
    {
        if (sliderMasterVolume != null)
        {
            float vol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            sliderMasterVolume.value = vol;
            AudioListener.volume = vol;
        }

        if (toggleFullscreen != null)
        {
            bool isFull = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            toggleFullscreen.isOn = isFull;
            Screen.fullScreen = isFull;
        }
    }

    public void ShowMainMenu()
    {
        SetAllPanels(false);
        if (panelMainButtons != null)
            panelMainButtons.SetActive(true);
    }

    public void ShowOptions()
    {
        SetAllPanels(false);
        if (panelOptions != null)
            panelOptions.SetActive(true);
    }

    public void ShowCredits()
    {
        SetAllPanels(false);
        if (panelCredits != null)
            panelCredits.SetActive(true);
    }

    public void ShowConfirmQuit()
    {
        SetAllPanels(false);
        if (panelConfirmQuit != null)
            panelConfirmQuit.SetActive(true);
    }

    private void SetAllPanels(bool state)
    {
        if (panelMainButtons != null) panelMainButtons.SetActive(state);
        if (panelOptions != null) panelOptions.SetActive(state);
        if (panelCredits != null) panelCredits.SetActive(state);
        if (panelConfirmQuit != null) panelConfirmQuit.SetActive(state);
    }

    // BOTONES PRINCIPALES

    public void OnPlayButton()
    {
        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void OnOptionsButton()
    {
        ShowOptions();
    }

    public void OnCreditsButton()
    {
        ShowCredits();
    }

    public void OnQuitButton()
    {
        ShowConfirmQuit();
    }

    // BOTONES DE OPCIONES / CRÉDITOS / QUIT

    public void OnBackFromOptions()
    {
        ShowMainMenu();
    }

    public void OnBackFromCredits()
    {
        ShowMainMenu();
    }

    public void OnQuitYes()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnQuitNo()
    {
        ShowMainMenu();
    }

    // EVENTOS DE OPCIONES

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }
}
