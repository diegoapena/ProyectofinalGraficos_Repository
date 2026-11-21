using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValue;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValue;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        playButton?.onClick.AddListener(PlayGame);
        optionsButton?.onClick.AddListener(OpenOptions);
        quitButton?.onClick.AddListener(QuitGame);
        backButton?.onClick.AddListener(BackToMain);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.02f;
            sensitivitySlider.maxValue = 0.40f;
            sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSensitivity", 0.12f));
            sensitivitySlider.onValueChanged.AddListener(HandleSensitivityChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", 1f));
            volumeSlider.onValueChanged.AddListener(HandleVolumeChanged);
        }

        HandleSensitivityChanged(PlayerPrefs.GetFloat("MouseSensitivity", 0.12f));
        HandleVolumeChanged(PlayerPrefs.GetFloat("MasterVolume", 1f));
        ShowMain();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("CityWorld");
    }

    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        ShowMain();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMain()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void HandleSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        if (sensitivityValue != null)
            sensitivityValue.text = value.ToString("0.00");
    }

    private void HandleVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        AudioListener.volume = value;

        if (volumeValue != null)
            volumeValue.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
