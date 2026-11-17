using System;
using UnityEngine;

public enum VisualStyle
{
    Toon,
    Realistic
}

public class VisualStyleManager : MonoBehaviour
{
    private const string PreferenceKey = "VisualStyle";

    public static VisualStyleManager Instance { get; private set; }

    public event Action<VisualStyle> StyleChanged;

    public VisualStyle CurrentStyle { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject host = new GameObject("VisualStyleManager");
        host.AddComponent<VisualStyleManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentStyle = (VisualStyle)PlayerPrefs.GetInt(PreferenceKey, (int)VisualStyle.Toon);
    }

    public void SetStyle(VisualStyle style)
    {
        CurrentStyle = style;
        PlayerPrefs.SetInt(PreferenceKey, (int)style);
        PlayerPrefs.Save();

        StyleChanged?.Invoke(style);
        StyleSwitchable.RefreshAll();
    }

    public void ToggleStyle()
    {
        SetStyle(CurrentStyle == VisualStyle.Toon ? VisualStyle.Realistic : VisualStyle.Toon);
    }
}
