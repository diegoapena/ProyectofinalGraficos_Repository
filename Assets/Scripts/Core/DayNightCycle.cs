using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }
    public static event System.Action<bool> OnPeriodChanged;

    [Header("Ciclo")]
    [SerializeField] private float cycleLengthSeconds = 180f;
    [SerializeField] private float startTimeOfDay = 0.25f;
    [SerializeField] private bool autoAdvance = true;

    [Header("Sol")]
    [SerializeField] private float sunAzimuth = 30f;
    [SerializeField] private float dayIntensity = 1.15f;

    [Header("Ambiente")]
    [SerializeField] private Color dayAmbient = new Color(0.42f, 0.46f, 0.52f);
    [SerializeField] private Color nightAmbient = new Color(0.10f, 0.12f, 0.20f);

    private Light sun;
    private float timeOfDay;

    public float TimeOfDay => timeOfDay;
    public bool IsDay { get; private set; }
    public float CycleLength => cycleLengthSeconds;

    public void SetTimeOfDay(float t)
    {
        timeOfDay = Mathf.Clamp01(t);
        ApplySun();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        Instance = this;
        timeOfDay = startTimeOfDay;
        IsDay = IsDaytime(timeOfDay);
        ApplyAmbient();
        OnPeriodChanged?.Invoke(IsDay);
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (!autoAdvance)
            return;

        timeOfDay += Time.deltaTime / cycleLengthSeconds;
        if (timeOfDay >= 1f)
            timeOfDay -= 1f;

        ApplySun();
    }

    private static bool IsDaytime(float t) => t >= 0.25f && t <= 0.75f;

    private void ApplySun()
    {
        if (sun == null)
        {
            sun = FindDirectionalLight();
            if (sun == null)
                return;
        }

        float rotationX = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(rotationX, sunAzimuth, 0f);

        float elevation = Mathf.Sin(rotationX * Mathf.Deg2Rad);

        bool isDay = IsDaytime(timeOfDay);
        if (isDay != IsDay)
        {
            IsDay = isDay;
            ApplyAmbient();
            OnPeriodChanged?.Invoke(IsDay);
        }

        if (isDay)
        {
            float f = Mathf.Clamp01(elevation);
            sun.intensity = Mathf.Lerp(0.15f, dayIntensity, f);
            sun.color = Color.Lerp(new Color(1f, 0.55f, 0.35f), new Color(1f, 0.97f, 0.92f), Mathf.Clamp01(elevation * 2.5f));
        }
        else
        {
            float f = Mathf.Clamp01(-elevation);
            sun.intensity = Mathf.Lerp(0.02f, 0.18f, f);
            sun.color = new Color(0.6f, 0.7f, 1f);
        }
    }

    private Light FindDirectionalLight()
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null && lights[i].type == LightType.Directional)
                return lights[i];
        }
        return null;
    }

    private void ApplyAmbient()
    {
        RenderSettings.ambientLight = IsDay ? dayAmbient : nightAmbient;
    }
}
