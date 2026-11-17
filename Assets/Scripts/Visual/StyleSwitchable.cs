using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StyleSwitchable : MonoBehaviour
{
    public Material toonMaterial;
    public Material realisticMaterial;

    private static readonly List<StyleSwitchable> registry = new List<StyleSwitchable>();

    private MeshRenderer cachedRenderer;

    public static IReadOnlyList<StyleSwitchable> All => registry;

    private void Awake()
    {
        cachedRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        if (cachedRenderer == null)
            cachedRenderer = GetComponent<MeshRenderer>();

        if (!registry.Contains(this))
            registry.Add(this);

        Apply();
    }

    private void OnDisable()
    {
        registry.Remove(this);
    }

    public void Apply()
    {
        if (cachedRenderer == null)
            cachedRenderer = GetComponent<MeshRenderer>();

        if (cachedRenderer == null)
            return;

        VisualStyle style = VisualStyleManager.Instance != null
            ? VisualStyleManager.Instance.CurrentStyle
            : (VisualStyle)PlayerPrefs.GetInt("VisualStyle", (int)VisualStyle.Toon);

        Material target = style == VisualStyle.Toon ? toonMaterial : realisticMaterial;

        if (target != null)
            cachedRenderer.sharedMaterial = target;
    }

    public void SetMaterials(Material toon, Material realistic)
    {
        toonMaterial = toon;
        realisticMaterial = realistic;
        Apply();
    }

    public static void RefreshAll()
    {
        for (int i = registry.Count - 1; i >= 0; i--)
        {
            if (registry[i] == null)
                registry.RemoveAt(i);
            else
                registry[i].Apply();
        }
    }
}
