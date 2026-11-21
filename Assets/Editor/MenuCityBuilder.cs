using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MenuCityBuilder
{
    [MenuItem("Tools/City/Rebuild Menu")]
    public static void Build()
    {
        DeleteObject("PreviewPivot");
        DeleteObject("Menu_Pedestal");
        DeleteObject("Menu_Halo");
        DeleteObject("Menu_HaloInner");

        BuildSkyline();

        GameObject title = FindObject("Title");
        if (title != null)
        {
            TextMeshProUGUI tmp = title.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "TOONCITY";
        }

        DeleteObject("StyleLabel");
        DeleteObject("ToonButton");
        DeleteObject("RealisticButton");
        DeleteObject("StyleStatus");

        SetupOptionsUI();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Menu actualizado correctamente.");
    }

    static void BuildSkyline()
    {
        Material mat = CreateSkylineMaterial();

        GameObject container = FindObject("Menu_Skyline");
        if (container == null)
        {
            container = new GameObject("Menu_Skyline");
        }

        float[] heights = { 11f, 16f, 9f, 18f, 13f, 20f, 8f, 15f, 12f, 17f, 10f, 14f, 19f, 9f, 15f, 12f, 18f, 11f };
        int n = heights.Length;
        float spacing = 2.0f;
        float startX = -((n - 1) * spacing) / 2f;
        float z = 12.3f;

        for (int i = 0; i < n; i++)
        {
            float x = startX + i * spacing;
            float h = heights[i];
            float w = 1.7f;
            float d = 3f;

            ProBuilderMesh pb = ShapeGenerator.GenerateCube(PivotLocation.Center, new Vector3(w, h, d));
            pb.name = "Skyline_" + (i + 1);
            pb.transform.SetParent(container.transform, false);
            pb.transform.position = new Vector3(x, h / 2f, z);
            pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
            pb.ToMesh();
            pb.Refresh();
            pb.gameObject.AddComponent<MeshCollider>();
            pb.gameObject.isStatic = true;
        }
    }

    static Material CreateSkylineMaterial()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials/City"))
            AssetDatabase.CreateFolder("Assets/Materials", "City");

        string path = "Assets/Materials/City/Skyline_Silueta.mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(shader) { name = "Skyline_Silueta" };
        m.SetColor("_BaseColor", new Color(0.05f, 0.08f, 0.13f));
        m.SetColor("_Color", new Color(0.05f, 0.08f, 0.13f));
        m.SetFloat("_Smoothness", 0.3f);
        m.SetFloat("_Metallic", 0.1f);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static void SetupOptionsUI()
    {
        RectTransform sensLabel = FindObject("SensitivityLabel")?.GetComponent<RectTransform>();
        RectTransform sensValue = FindObject("SensitivityValue")?.GetComponent<RectTransform>();
        RectTransform sensSlider = FindObject("SensitivitySlider")?.GetComponent<RectTransform>();

        if (sensLabel != null) sensLabel.anchoredPosition = new Vector2(-85f, 170f);
        if (sensValue != null) sensValue.anchoredPosition = new Vector2(215f, 170f);
        if (sensSlider != null) sensSlider.anchoredPosition = new Vector2(0f, 115f);

        GameObject volLabel = Duplicate("SensitivityLabel", "VolumeLabel");
        GameObject volValue = Duplicate("SensitivityValue", "VolumeValue");
        GameObject volSlider = Duplicate("SensitivitySlider", "VolumeSlider");

        if (volLabel != null)
        {
            volLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-85f, 30f);
            TextMeshProUGUI tmp = volLabel.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "VOLUMEN";
        }

        if (volValue != null)
        {
            volValue.GetComponent<RectTransform>().anchoredPosition = new Vector2(215f, 30f);
            TextMeshProUGUI tmp = volValue.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "100%";
        }

        if (volSlider != null)
        {
            volSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -25f);
        }

        GameObject canvasObj = FindObject("MainMenuCanvas");
        if (canvasObj == null) return;

        MainMenuController controller = canvasObj.GetComponent<MainMenuController>();
        if (controller == null) return;

        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("volumeSlider").objectReferenceValue = volSlider != null ? volSlider.GetComponent<Slider>() : null;
        so.FindProperty("volumeValue").objectReferenceValue = volValue != null ? volValue.GetComponent<TextMeshProUGUI>() : null;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static GameObject Duplicate(string sourceName, string newName)
    {
        GameObject source = FindObject(sourceName);
        if (source == null) return null;

        GameObject clone = (GameObject)Object.Instantiate(source, source.transform.parent);
        clone.name = newName;
        return clone;
    }

    static void DeleteObject(string name)
    {
        GameObject obj = FindObject(name);
        if (obj != null)
            Object.DestroyImmediate(obj);
    }

    static GameObject FindObject(string name)
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            GameObject found = FindRecursive(root.transform, name);
            if (found != null) return found;
        }
        return null;
    }

    static GameObject FindRecursive(Transform t, string name)
    {
        if (t.name == name) return t.gameObject;
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject found = FindRecursive(t.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}
