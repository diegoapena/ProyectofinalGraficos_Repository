using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEditor;

public static class TrafficBuilder
{
    static Material npcPiel, npcRopa1, npcRopa2, npcRopa3, vidrio, metalUrbano, carroRojo, carroAzul, carroBlanco, carroAmarillo, carroGris;

    [MenuItem("Tools/City/Add Traffic to City")]
    public static void AddCityTraffic()
    {
        LoadMaterials();
        ClearTraffic();

        GameObject folder = NewFolder("Trafico");
        GameObject wayFolder = NewFolder("Waypoints");

        // ---- Carros en movimiento ----
        Car(folder, wayFolder, "Trafico_Avenida_1", new Vector3(3f, 0.06f, -95f), new Vector3[] { new Vector3(3f, 0.06f, -95f), new Vector3(3f, 0.06f, 110f) }, carroRojo, 8f);
        Car(folder, wayFolder, "Trafico_Avenida_2", new Vector3(-3f, 0.06f, 110f), new Vector3[] { new Vector3(-3f, 0.06f, 110f), new Vector3(-3f, 0.06f, -95f) }, carroAzul, 7f);
        Car(folder, wayFolder, "Trafico_Centro_1", new Vector3(-110f, 0.06f, 0f), new Vector3[] { new Vector3(-110f, 0.06f, 0f), new Vector3(110f, 0.06f, 0f) }, carroBlanco, 8f);
        Car(folder, wayFolder, "Trafico_Centro_2", new Vector3(110f, 0.06f, 0f), new Vector3[] { new Vector3(110f, 0.06f, 0f), new Vector3(-110f, 0.06f, 0f) }, carroGris, 7f);
        Car(folder, wayFolder, "Trafico_Este", new Vector3(63f, 0.06f, -95f), new Vector3[] { new Vector3(63f, 0.06f, -95f), new Vector3(63f, 0.06f, 110f) }, carroAmarillo, 6.5f);
        Bus(folder, wayFolder, "Trafico_Bus", new Vector3(-110f, 0.06f, 63f), new Vector3[] { new Vector3(-110f, 0.06f, 63f), new Vector3(110f, 0.06f, 63f) }, 5f);

        // ---- NPCs ----
        Npc(folder, wayFolder, "NPC_Av_1", new Vector3(8.75f, 0.16f, -80f), new Vector3[] { new Vector3(8.75f, 0.16f, -80f), new Vector3(8.75f, 0.16f, 80f) }, npcRopa1, 1.6f);
        Npc(folder, wayFolder, "NPC_Av_2", new Vector3(-8.75f, 0.16f, 80f), new Vector3[] { new Vector3(-8.75f, 0.16f, 80f), new Vector3(-8.75f, 0.16f, -80f) }, npcRopa2, 1.7f);
        Npc(folder, wayFolder, "NPC_Av_3", new Vector3(8.75f, 0.16f, 40f), new Vector3[] { new Vector3(8.75f, 0.16f, 40f), new Vector3(8.75f, 0.16f, -40f) }, npcRopa3, 1.4f);
        Npc(folder, wayFolder, "NPC_Centro_1", new Vector3(-50f, 0.16f, -4.5f), new Vector3[] { new Vector3(-50f, 0.16f, -4.5f), new Vector3(50f, 0.16f, -4.5f) }, npcRopa1, 1.5f);
        Npc(folder, wayFolder, "NPC_Centro_2", new Vector3(50f, 0.16f, 4.5f), new Vector3[] { new Vector3(50f, 0.16f, 4.5f), new Vector3(-50f, 0.16f, 4.5f) }, npcRopa2, 1.6f);
        Npc(folder, wayFolder, "NPC_Parque_1", new Vector3(20f, 0.05f, 25f), new Vector3[] { new Vector3(20f, 0.05f, 25f), new Vector3(48f, 0.05f, 25f), new Vector3(48f, 0.05f, 40f), new Vector3(20f, 0.05f, 40f) }, npcRopa3, 1.5f);
        Npc(folder, wayFolder, "NPC_Parque_2", new Vector3(33.5f, 0.05f, 10f), new Vector3[] { new Vector3(33.5f, 0.05f, 10f), new Vector3(33.5f, 0.05f, 50f) }, npcRopa1, 1.4f);
        Npc(folder, wayFolder, "NPC_Plaza", new Vector3(-75f, 0.16f, 96f), new Vector3[] { new Vector3(-75f, 0.16f, 96f), new Vector3(-90f, 0.16f, 104f), new Vector3(-70f, 0.16f, 110f) }, npcRopa2, 1.5f);

        Debug.Log("Tráfico y NPCs añadidos a la ciudad.");
    }

    [MenuItem("Tools/City/Add Traffic to Menu")]
    public static void AddMenuTraffic()
    {
        LoadMaterials();
        ClearTraffic();

        GameObject folder = NewFolder("Trafico");
        GameObject wayFolder = NewFolder("Waypoints");

        // Carril en el piso del menú
        Box(folder.transform, "Carril", new Vector3(0f, -0.03f, 5f), new Vector3(30f, 0.06f, 3f), carroGris, true);

        // Carros en el menú
        Car(folder, wayFolder, "Menu_Carro_1", new Vector3(-14f, -0.02f, 5f), new Vector3[] { new Vector3(-14f, -0.02f, 5f), new Vector3(14f, -0.02f, 5f) }, carroRojo, 6f);
        Car(folder, wayFolder, "Menu_Carro_2", new Vector3(14f, -0.02f, 5f), new Vector3[] { new Vector3(14f, -0.02f, 5f), new Vector3(-14f, -0.02f, 5f) }, carroAzul, 5f);

        // NPCs en el menú
        Npc(folder, wayFolder, "Menu_NPC_1", new Vector3(-10f, -0.04f, 2f), new Vector3[] { new Vector3(-10f, -0.04f, 2f), new Vector3(10f, -0.04f, 2f) }, npcRopa1, 1.4f);
        Npc(folder, wayFolder, "Menu_NPC_2", new Vector3(10f, -0.04f, 8f), new Vector3[] { new Vector3(10f, -0.04f, 8f), new Vector3(-10f, -0.04f, 8f) }, npcRopa2, 1.3f);
        Npc(folder, wayFolder, "Menu_NPC_3", new Vector3(0f, -0.04f, 3.5f), new Vector3[] { new Vector3(0f, -0.04f, 3.5f), new Vector3(12f, -0.04f, 3.5f), new Vector3(-12f, -0.04f, 6.5f) }, npcRopa3, 1.5f);

        Debug.Log("Tráfico y NPCs añadidos al menú.");
    }

    static void ClearTraffic()
    {
        GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = roots.Length - 1; i >= 0; i--)
        {
            if (roots[i].name == "Trafico" || roots[i].name == "Waypoints")
                Object.DestroyImmediate(roots[i]);
        }
    }

    static void LoadMaterials()
    {
        npcPiel = GetOrCreateMat("NPC_Piel", new Color(0.85f, 0.70f, 0.60f), 0.4f, 0f);
        npcRopa1 = GetOrCreateMat("NPC_Ropa_1", new Color(0.18f, 0.32f, 0.52f), 0.4f, 0f);
        npcRopa2 = GetOrCreateMat("NPC_Ropa_2", new Color(0.55f, 0.20f, 0.20f), 0.4f, 0f);
        npcRopa3 = GetOrCreateMat("NPC_Ropa_3", new Color(0.30f, 0.42f, 0.28f), 0.4f, 0f);
        vidrio = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Vidrio_Ventana.mat");
        metalUrbano = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Urbano.mat");
        carroRojo = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Vehiculo_Rojo.mat");
        carroAzul = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Vehiculo_Azul.mat");
        carroBlanco = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Vehiculo_Blanco.mat");
        carroAmarillo = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Vehiculo_Amarillo.mat");
        carroGris = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/City/Metal_Vehiculo.mat");
    }

    static Material GetOrCreateMat(string name, Color color, float smoothness, float metallic)
    {
        string path = "Assets/Materials/City/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        if (!AssetDatabase.IsValidFolder("Assets/Materials/City"))
            AssetDatabase.CreateFolder("Assets/Materials", "City");

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(shader) { name = name };
        m.SetColor("_BaseColor", color);
        m.SetColor("_Color", color);
        m.SetFloat("_Smoothness", smoothness);
        m.SetFloat("_Metallic", metallic);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static GameObject NewFolder(string name)
    {
        GameObject existing = null;
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == name) { existing = root; break; }
        }
        if (existing != null) return existing;
        return new GameObject(name);
    }

    static void Box(Transform parent, string name, Vector3 pos, Vector3 size, Material mat, bool isStatic)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = pos;
        pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        if (isStatic) pb.gameObject.AddComponent<MeshCollider>();
        pb.gameObject.isStatic = isStatic;
    }

    static void Cylinder(Transform parent, string name, Vector3 localPos, float radius, float height, Material mat)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateCylinder(PivotLocation.Center, 12, radius, height, 1, 1);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = localPos;
        pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        pb.gameObject.isStatic = false;
    }

    static void Sphere(Transform parent, string name, Vector3 localPos, float radius, Material mat)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, radius, 1, true, false);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = localPos;
        pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        pb.gameObject.isStatic = false;
    }

    static void Wheel(Transform parent, string name, Vector3 localPos, float radius)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateCylinder(PivotLocation.Center, 12, radius, 0.3f, 1, 1);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = localPos;
        pb.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        pb.GetComponent<MeshRenderer>().sharedMaterial = metalUrbano;
        pb.ToMesh();
        pb.Refresh();
        pb.gameObject.isStatic = false;
    }

    static GameObject Car(GameObject folder, GameObject wayFolder, string name, Vector3 pos, Vector3[] points, Material body, float speed)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(folder.transform, false);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        Transform p = go.transform;
        Box(p, "Carroceria", new Vector3(0f, 0.55f, 0f), new Vector3(1.8f, 0.5f, 4.2f), body, false);
        Box(p, "Cabina", new Vector3(0f, 1.15f, -0.2f), new Vector3(1.6f, 0.7f, 2.4f), body, false);
        Box(p, "Parabrisas", new Vector3(0f, 1.2f, 0.9f), new Vector3(1.5f, 0.6f, 0.06f), vidrio, false);
        Box(p, "VentanaL", new Vector3(0.81f, 1.2f, -0.2f), new Vector3(0.06f, 0.6f, 1.8f), vidrio, false);
        Box(p, "VentanaR", new Vector3(-0.81f, 1.2f, -0.2f), new Vector3(0.06f, 0.6f, 1.8f), vidrio, false);
        Box(p, "FaroL", new Vector3(0.55f, 0.6f, 2.1f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, false);
        Box(p, "FaroR", new Vector3(-0.55f, 0.6f, 2.1f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, false);
        Wheel(p, "Rueda_FL", new Vector3(0.75f, 0.35f, 1.35f), 0.35f);
        Wheel(p, "Rueda_FR", new Vector3(-0.75f, 0.35f, 1.35f), 0.35f);
        Wheel(p, "Rueda_RL", new Vector3(0.75f, 0.35f, -1.35f), 0.35f);
        Wheel(p, "Rueda_RR", new Vector3(-0.75f, 0.35f, -1.35f), 0.35f);

        AttachMover(go, wayFolder, points, speed, false);
        return go;
    }

    static GameObject Bus(GameObject folder, GameObject wayFolder, string name, Vector3 pos, Vector3[] points, float speed)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(folder.transform, false);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        Transform p = go.transform;
        Box(p, "Cuerpo", new Vector3(0f, 1.4f, 0f), new Vector3(2.5f, 1.6f, 12f), carroAmarillo, false);
        Box(p, "Techo", new Vector3(0f, 2.6f, 0f), new Vector3(2.4f, 0.4f, 12f), carroAmarillo, false);
        Box(p, "VentanasL", new Vector3(1.26f, 1.9f, 0f), new Vector3(0.05f, 0.7f, 11f), vidrio, false);
        Box(p, "VentanasR", new Vector3(-1.26f, 1.9f, 0f), new Vector3(0.05f, 0.7f, 11f), vidrio, false);
        Wheel(p, "Rueda_FL", new Vector3(1.05f, 0.55f, 4.2f), 0.55f);
        Wheel(p, "Rueda_FR", new Vector3(-1.05f, 0.55f, 4.2f), 0.55f);
        Wheel(p, "Rueda_RL", new Vector3(1.05f, 0.55f, -4.2f), 0.55f);
        Wheel(p, "Rueda_RR", new Vector3(-1.05f, 0.55f, -4.2f), 0.55f);

        AttachMover(go, wayFolder, points, speed, false);
        return go;
    }

    static GameObject Npc(GameObject folder, GameObject wayFolder, string name, Vector3 pos, Vector3[] points, Material clothes, float speed)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(folder.transform, false);
        go.transform.position = pos;

        Transform p = go.transform;
        Cylinder(p, "Cuerpo", new Vector3(0f, 0.7f, 0f), 0.34f, 1.4f, clothes);
        Sphere(p, "Cabeza", new Vector3(0f, 1.66f, 0f), 0.26f, npcPiel);

        AttachMover(go, wayFolder, points, speed, false);
        return go;
    }

    static void AttachMover(GameObject go, GameObject wayFolder, Vector3[] points, float speed, bool loop)
    {
        WaypointMover mover = go.AddComponent<WaypointMover>();
        mover.speed = speed;
        mover.loop = loop;
        mover.faceDirection = true;

        Transform[] wps = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            GameObject wp = new GameObject(go.name + "_wp" + i);
            wp.transform.SetParent(wayFolder.transform, false);
            wp.transform.position = points[i];
            wps[i] = wp.transform;
        }
        mover.waypoints = wps;
    }
}
