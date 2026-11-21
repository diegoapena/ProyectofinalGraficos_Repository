using UnityEngine;

public class NightEmissive : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private Color nightColor = new Color(1f, 0.85f, 0.55f);
    [SerializeField] private float emissionIntensity = 2.2f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        DayNightCycle.OnPeriodChanged += HandlePeriod;
        HandlePeriod(DayNightCycle.Instance != null ? DayNightCycle.Instance.IsDay : true);
    }

    private void OnDisable()
    {
        DayNightCycle.OnPeriodChanged -= HandlePeriod;
    }

    private void HandlePeriod(bool isDay)
    {
        Apply(isDay);
    }

    private void Apply(bool isDay)
    {
        if (materials == null)
            return;

        for (int i = 0; i < materials.Length; i++)
        {
            Material m = materials[i];
            if (m == null)
                continue;

            if (isDay)
            {
                m.DisableKeyword("_EMISSION");
                m.SetColor(EmissionColorId, Color.black);
            }
            else
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor(EmissionColorId, nightColor * emissionIntensity);
            }
        }
    }
}
