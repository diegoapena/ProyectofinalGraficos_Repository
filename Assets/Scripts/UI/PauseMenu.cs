using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PlayerCarry carry;

    private bool paused;

    public bool IsPaused => paused;

    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        resumeButton?.onClick.AddListener(Resume);
        restartButton?.onClick.AddListener(RestartLevel);
        menuButton?.onClick.AddListener(GoToMenu);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame)
            return;

        if (GameplayHUD.Instance != null && GameplayHUD.Instance.IsVictoryVisible)
            return;

        SetPaused(!paused);
    }

    public void SetPaused(bool value)
    {
        paused = value;

        if (pausePanel != null)
            pausePanel.SetActive(value);

        Time.timeScale = value ? 0f : 1f;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;

        if (controller != null) controller.enabled = !value;
        if (interactor != null) interactor.enabled = !value;
        if (carry != null) carry.enabled = !value;
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CityWorld");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (paused)
            Time.timeScale = 1f;
    }
}
