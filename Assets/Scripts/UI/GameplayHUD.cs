using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayHUD : MonoBehaviour
{
    public static GameplayHUD Instance { get; private set; }

    [Header("Crosshair")]
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Image[] crosshairParts;
    [SerializeField] private Color crosshairIdleColor = new Color(1f, 1f, 1f, 0.85f);
    [SerializeField] private Color crosshairHoverColor = new Color(0.20f, 0.95f, 0.95f, 1f);
    [SerializeField] private float crosshairHoverScale = 1.35f;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionCard;
    [SerializeField] private TMP_Text interactionLabel;

    [Header("Charge")]
    [SerializeField] private GameObject chargeCard;
    [SerializeField] private Image chargeFill;
    [SerializeField] private TMP_Text chargeValue;

    [Header("Victory")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button victoryMenuButton;
    [SerializeField] private Button victoryRestartButton;

    [Header("References")]
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PlayerCarry carry;
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private VictoryZone victoryZone;

    public bool IsVictoryVisible => victoryPanel != null && victoryPanel.activeSelf;

    private void Awake()
    {
        Instance = this;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (interactionCard != null)
            interactionCard.SetActive(false);

        if (chargeCard != null)
            chargeCard.SetActive(false);

        victoryMenuButton?.onClick.AddListener(GoToMenu);
        victoryRestartButton?.onClick.AddListener(RestartLevel);
    }

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.OnHoverStart += HandleHoverStart;
            interactor.OnHoverEnd += HandleHoverEnd;
        }

        if (carry != null)
            carry.OnChargeChanged += HandleChargeChanged;

        if (victoryZone != null)
            victoryZone.OnVictoryEvent.AddListener(ShowVictory);
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.OnHoverStart -= HandleHoverStart;
            interactor.OnHoverEnd -= HandleHoverEnd;
        }

        if (carry != null)
            carry.OnChargeChanged -= HandleChargeChanged;

        if (victoryZone != null)
            victoryZone.OnVictoryEvent.RemoveListener(ShowVictory);
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
        {
            if (crosshair != null) crosshair.gameObject.SetActive(false);
            if (interactionCard != null) interactionCard.SetActive(false);
            if (chargeCard != null) chargeCard.SetActive(false);
            return;
        }

        if (crosshair != null && !crosshair.gameObject.activeSelf)
            crosshair.gameObject.SetActive(true);

        bool hasFocus = interactor != null && interactor.Focused != null;

        if (crosshair != null)
        {
            float scale = hasFocus ? crosshairHoverScale : 1f;
            crosshair.localScale = Vector3.Lerp(crosshair.localScale, Vector3.one * scale, Time.unscaledDeltaTime * 12f);
        }

        if (crosshairParts != null)
        {
            Color target = hasFocus ? crosshairHoverColor : crosshairIdleColor;
            for (int i = 0; i < crosshairParts.Length; i++)
            {
                if (crosshairParts[i] != null)
                    crosshairParts[i].color = Color.Lerp(crosshairParts[i].color, target, Time.unscaledDeltaTime * 12f);
            }
        }

        if (interactionLabel != null)
            interactionLabel.text = hasFocus ? interactor.Focused.InteractionPrompt : string.Empty;

        if (chargeCard != null)
            chargeCard.SetActive(carry != null && carry.IsCharging);

        if (carry != null && carry.IsCharging)
        {
            if (chargeFill != null)
                chargeFill.fillAmount = carry.ChargeNormalized;

            if (chargeValue != null)
                chargeValue.text = Mathf.RoundToInt(carry.CurrentPower).ToString();
        }
    }

    private void HandleHoverStart(IInteractable target)
    {
        if (interactionCard != null)
            interactionCard.SetActive(true);

        if (interactionLabel != null && target != null)
            interactionLabel.text = target.InteractionPrompt;
    }

    private void HandleHoverEnd(IInteractable target)
    {
        if (interactionCard != null)
            interactionCard.SetActive(false);
    }

    private void HandleChargeChanged(float normalized)
    {
        if (chargeCard != null)
            chargeCard.SetActive(normalized > 0.0001f);

        if (chargeFill != null)
            chargeFill.fillAmount = normalized;

        if (chargeValue != null && carry != null)
            chargeValue.text = Mathf.RoundToInt(carry.CurrentPower).ToString();
    }

    public void ShowVictory()
    {
        if (victoryPanel == null)
            return;

        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (controller != null) controller.enabled = false;
        if (interactor != null) interactor.enabled = false;
        if (carry != null) carry.enabled = false;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CityWorld");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (IsVictoryVisible)
            Time.timeScale = 1f;
    }
}
