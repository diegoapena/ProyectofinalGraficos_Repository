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
    [SerializeField] private Button toonButton;
    [SerializeField] private Button realisticButton;

    [Header("Style")]
    [SerializeField] private Image toonButtonBackground;
    [SerializeField] private Image realisticButtonBackground;
    [SerializeField] private TMP_Text styleStatus;
    [SerializeField] private Color selectedColor = new Color(0.10f, 0.66f, 0.68f, 1f);
    [SerializeField] private Color idleColor = new Color(0.12f, 0.13f, 0.18f, 0.95f);

    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValue;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        playButton?.onClick.AddListener(PlayGame);
        optionsButton?.onClick.AddListener(OpenOptions);
        quitButton?.onClick.AddListener(QuitGame);
        backButton?.onClick.AddListener(BackToMain);
        toonButton?.onClick.AddListener(SelectToon);
        realisticButton?.onClick.AddListener(SelectRealistic);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.02f;
            sensitivitySlider.maxValue = 0.40f;
            sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSensitivity", 0.12f));
            sensitivitySlider.onValueChanged.AddListener(HandleSensitivityChanged);
        }

        HandleSensitivityChanged(PlayerPrefs.GetFloat("MouseSensitivity", 0.12f));
        ShowMain();
        RefreshStyleUI();
    }

    private void OnEnable()
    {
        if (VisualStyleManager.Instance != null)
            VisualStyleManager.Instance.StyleChanged += HandleStyleChanged;
    }

    private void OnDisable()
    {
        if (VisualStyleManager.Instance != null)
            VisualStyleManager.Instance.StyleChanged -= HandleStyleChanged;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainLevels");
    }

    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        RefreshStyleUI();
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

    public void SelectToon()
    {
        VisualStyleManager.Instance?.SetStyle(VisualStyle.Toon);
        RefreshStyleUI();
    }

    public void SelectRealistic()
    {
        VisualStyleManager.Instance?.SetStyle(VisualStyle.Realistic);
        RefreshStyleUI();
    }

    private void ShowMain()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void HandleStyleChanged(VisualStyle style)
    {
        RefreshStyleUI();
    }

    private void RefreshStyleUI()
    {
        VisualStyle current = VisualStyleManager.Instance != null
            ? VisualStyleManager.Instance.CurrentStyle
            : (VisualStyle)PlayerPrefs.GetInt("VisualStyle", (int)VisualStyle.Toon);

        if (toonButtonBackground != null)
            toonButtonBackground.color = current == VisualStyle.Toon ? selectedColor : idleColor;

        if (realisticButtonBackground != null)
            realisticButtonBackground.color = current == VisualStyle.Realistic ? selectedColor : idleColor;

        if (styleStatus != null)
            styleStatus.text = current == VisualStyle.Toon ? "Estilo actual: Toon (Cel Shading)" : "Estilo actual: Realista (PBR)";
    }

    private void HandleSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        if (sensitivityValue != null)
            sensitivityValue.text = value.ToString("0.00");
    }
}
