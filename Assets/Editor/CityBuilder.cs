using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CityBuilder
{
    const string ScenePath = "Assets/Scenes/CityWorld.unity";
    const string MatFolder = "Assets/Materials/City";

    static Material asfalto, acera, concreto, vidrio, vidrioEdificio, emisivoFarola, cesped, corteza, follaje, agua, metalUrbano, metalVehiculo, puente, carroRojo, carroAzul, carroBlanco, carroAmarillo;

    [MenuItem("Tools/City/Generate CityWorld")]
    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientLight = new Color(0.42f, 0.46f, 0.52f);
        RenderSettings.fog = false;

        CreateMaterials();

        GameObject folderTerreno = NewGO("01_Terreno_Vias", Vector3.zero, null);
        GameObject folderPuentes = NewGO("02_Puentes", Vector3.zero, null);
        GameObject folderEdificios = NewGO("03_Edificios", Vector3.zero, null);
        GameObject folderParque = NewGO("04_Parque_Arboles", Vector3.zero, null);
        GameObject folderVehiculos = NewGO("05_Vehiculos", Vector3.zero, null);
        GameObject folderMobiliario = NewGO("06_Mobiliario_Urbano", Vector3.zero, null);
        GameObject folderJugador = NewGO("07_Jugador_UI", Vector3.zero, null);
        GameObject folderLuz = NewGO("00_Iluminacion", Vector3.zero, null);

        BuildTerrainAndRoads(folderTerreno.transform);
        BuildBridges(folderPuentes.transform);
        BuildBuildings(folderEdificios.transform);
        BuildPark(folderParque.transform);
        BuildVehicles(folderVehiculos.transform);
        BuildUrbanFurniture(folderMobiliario.transform);
        BuildLighting(folderLuz.transform);
        BuildInteractivePlaza(folderTerreno.transform);

        ImportPlayer(scene, folderJugador.transform);

        EditorSceneManager.SaveScene(scene, ScenePath);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene(ScenePath, true),
        };

        Debug.Log("CityWorld generada correctamente en " + ScenePath);
    }

    static GameObject NewGO(string name, Vector3 pos, Transform parent)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.position = pos;
        return go;
    }

    static void MeshBox(string name, Vector3 center, Vector3 size, Material mat, Transform parent, bool collider = true)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = center;
        if (mat != null) pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        if (collider) pb.gameObject.AddComponent<MeshCollider>();
        pb.gameObject.isStatic = true;
    }

    static void MeshCylinder(string name, Vector3 center, float radius, float height, Material mat, Transform parent, int divisions = 16, bool collider = true)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateCylinder(PivotLocation.Center, divisions, radius, height, 1, 1);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = center;
        if (mat != null) pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        if (collider) pb.gameObject.AddComponent<MeshCollider>();
        pb.gameObject.isStatic = true;
    }

    static void MeshSphere(string name, Vector3 center, float radius, Material mat, Transform parent, bool collider = true)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, radius, 1, true, false);
        pb.name = name;
        pb.transform.SetParent(parent, false);
        pb.transform.localPosition = center;
        if (mat != null) pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
        if (collider) pb.gameObject.AddComponent<MeshCollider>();
        pb.gameObject.isStatic = true;
    }

    static void CreateMaterials()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials/City"))
            AssetDatabase.CreateFolder("Assets/Materials", "City");

        asfalto = Mat("Asfalto", new Color(0.13f, 0.13f, 0.14f), 0.25f, 0f);
        acera = Mat("Acera", new Color(0.55f, 0.56f, 0.58f), 0.3f, 0f);
        concreto = Mat("Concreto_Edificio", new Color(0.62f, 0.63f, 0.66f), 0.2f, 0f);
        vidrio = Mat("Vidrio_Ventana", new Color(0.13f, 0.22f, 0.30f), 0.9f, 0.1f);
        vidrioEdificio = Mat("Vidrio_Edificio", new Color(0.13f, 0.22f, 0.30f), 0.9f, 0.1f);
        emisivoFarola = Mat("Emisivo_Farola", new Color(0.35f, 0.32f, 0.26f), 0.5f, 0.2f);
        cesped = Mat("Cesped", new Color(0.26f, 0.46f, 0.20f), 0.05f, 0f);
        corteza = Mat("Corteza_Arbol", new Color(0.40f, 0.28f, 0.16f), 0.2f, 0f);
        follaje = Mat("Follaje", new Color(0.20f, 0.52f, 0.22f), 0.05f, 0f);
        agua = Mat("Agua_Rio", new Color(0.15f, 0.35f, 0.52f), 0.9f, 0f);
        metalUrbano = Mat("Metal_Urbano", new Color(0.26f, 0.28f, 0.31f), 0.6f, 0.6f);
        metalVehiculo = Mat("Metal_Vehiculo", new Color(0.35f, 0.36f, 0.38f), 0.7f, 0.5f);
        puente = Mat("Concreto_Puente", new Color(0.58f, 0.59f, 0.62f), 0.25f, 0f);
        carroRojo = Mat("Metal_Vehiculo_Rojo", new Color(0.65f, 0.15f, 0.14f), 0.7f, 0.5f);
        carroAzul = Mat("Metal_Vehiculo_Azul", new Color(0.14f, 0.25f, 0.55f), 0.7f, 0.5f);
        carroBlanco = Mat("Metal_Vehiculo_Blanco", new Color(0.85f, 0.85f, 0.86f), 0.7f, 0.5f);
        carroAmarillo = Mat("Metal_Vehiculo_Amarillo", new Color(0.85f, 0.72f, 0.10f), 0.7f, 0.5f);
    }

    static Material Mat(string name, Color color, float smoothness, float metallic)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(shader) { name = name };
        m.SetColor("_BaseColor", color);
        m.SetColor("_Color", color);
        m.SetFloat("_Smoothness", smoothness);
        m.SetFloat("_Metallic", metallic);
        AssetDatabase.CreateAsset(m, MatFolder + "/" + name + ".mat");
        return m;
    }

    static void Road(float xMin, float xMax, float zMin, float zMax, Transform parent)
    {
        float cx = (xMin + xMax) / 2f;
        float cz = (zMin + zMax) / 2f;
        float w = xMax - xMin;
        float d = zMax - zMin;
        MeshBox("Asfalto", new Vector3(cx, 0.0f, cz), new Vector3(w, 0.08f, d), asfalto, parent);
    }

    static void Sidewalk(float xMin, float xMax, float zMin, float zMax, Transform parent)
    {
        float cx = (xMin + xMax) / 2f;
        float cz = (zMin + zMax) / 2f;
        float w = xMax - xMin;
        float d = zMax - zMin;
        MeshBox("Acera", new Vector3(cx, 0.08f, cz), new Vector3(w, 0.2f, d), acera, parent);
    }

    static void BuildTerrainAndRoads(Transform t)
    {
        MeshBox("Suelo_Cesped", new Vector3(0f, -0.5f, 0f), new Vector3(300f, 1f, 300f), cesped, t);

        Road(-7f, 7f, -140f, -120f, t);   // Avenida Central (sur)
        Road(-7f, 7f, -106f, 140f, t);    // Avenida Central (norte)
        Road(-140f, 140f, -66f, -60f, t); // Calle Norte
        Road(-140f, 140f, -3f, 3f, t);    // Calle Centro
        Road(-140f, 140f, 60f, 66f, t);   // Calle Sur
        Road(-66f, -60f, -140f, -120f, t); // Calle Oeste (sur)
        Road(-66f, -60f, -106f, 140f, t);  // Calle Oeste (norte)
        Road(60f, 66f, -140f, -120f, t);   // Calle Este (sur)
        Road(60f, 66f, -106f, 140f, t);    // Calle Este (norte)

        // Aceras Avenida (segmentadas por cruces y por el río)
        Sidewalk(7f, 10.5f, -140f, -120f, t);
        Sidewalk(7f, 10.5f, -106f, -66f, t);
        Sidewalk(7f, 10.5f, -60f, -3f, t);
        Sidewalk(7f, 10.5f, 3f, 60f, t);
        Sidewalk(7f, 10.5f, 66f, 140f, t);
        Sidewalk(-10.5f, -7f, -140f, -120f, t);
        Sidewalk(-10.5f, -7f, -106f, -66f, t);
        Sidewalk(-10.5f, -7f, -60f, -3f, t);
        Sidewalk(-10.5f, -7f, 3f, 60f, t);
        Sidewalk(-10.5f, -7f, 66f, 140f, t);

        // Aceras de calles E-W
        Sidewalk(-140f, -66f, -69f, -66f, t);
        Sidewalk(-60f, -7f, -69f, -66f, t);
        Sidewalk(7f, 60f, -69f, -66f, t);
        Sidewalk(66f, 140f, -69f, -66f, t);

        Sidewalk(-140f, -66f, -6f, -3f, t);
        Sidewalk(-60f, -7f, -6f, -3f, t);
        Sidewalk(7f, 60f, -6f, -3f, t);
        Sidewalk(66f, 140f, -6f, -3f, t);

        Sidewalk(-140f, -66f, 60f, 63f, t);
        Sidewalk(-60f, -7f, 60f, 63f, t);
        Sidewalk(7f, 60f, 60f, 63f, t);
        Sidewalk(66f, 140f, 60f, 63f, t);

        // Aceras de calles N-S
        Sidewalk(-69f, -66f, -140f, -120f, t);
        Sidewalk(-69f, -66f, -106f, -66f, t);
        Sidewalk(-69f, -66f, -60f, -3f, t);
        Sidewalk(-69f, -66f, 3f, 60f, t);
        Sidewalk(-69f, -66f, 66f, 140f, t);
        Sidewalk(60f, 63f, -140f, -120f, t);
        Sidewalk(60f, 63f, -106f, -66f, t);
        Sidewalk(60f, 63f, -60f, -3f, t);
        Sidewalk(60f, 63f, 3f, 60f, t);
        Sidewalk(60f, 63f, 66f, 140f, t);

        BuildRiver(t);
    }

    static void BuildRiver(Transform t)
    {
        // Agua (canal urbano, ligeramente hundido)
        MeshBox("Rio_Agua", new Vector3(0f, -0.22f, -113f), new Vector3(280f, 0.16f, 14f), agua, t);
    }

    static void BuildBridges(Transform t)
    {
        BuildConcreteBridge(t, 0f, 14f);   // Avenida Central
        BuildConcreteBridge(t, 63f, 6f);   // Calle Este
        BuildConcreteBridge(t, -63f, 6f);  // Calle Oeste
        BuildSuspensionBridge(t, -30f);
    }

    static void BuildConcreteBridge(Transform t, float xCenter, float width)
    {
        float zc = -113f;
        // Tablero a nivel de calzada (conecta ambos lados del río)
        MeshBox("Puente_Deck", new Vector3(xCenter, -0.02f, zc), new Vector3(width, 0.12f, 14f), puente, t);
        // Barandas laterales
        MeshBox("Baranda_L", new Vector3(xCenter - width / 2f + 0.2f, 0.6f, zc), new Vector3(0.25f, 1.2f, 14f), metalUrbano, t);
        MeshBox("Baranda_R", new Vector3(xCenter + width / 2f - 0.2f, 0.6f, zc), new Vector3(0.25f, 1.2f, 14f), metalUrbano, t);
    }

    static void BuildSuspensionBridge(Transform t, float xCenter)
    {
        float zc = -113f;
        float w = 5f;
        MeshBox("Puente_Colgante_Deck", new Vector3(xCenter, -0.02f, zc), new Vector3(w, 0.12f, 14f), puente, t);
        MeshBox("Torre_L", new Vector3(xCenter - 2.2f, 2.2f, zc), new Vector3(0.5f, 4.4f, 0.5f), metalUrbano, t);
        MeshBox("Torre_R", new Vector3(xCenter + 2.2f, 2.2f, zc), new Vector3(0.5f, 4.4f, 0.5f), metalUrbano, t);
        MeshBox("Cable_Top_L", new Vector3(xCenter - 2.2f, 4.4f, zc), new Vector3(0.12f, 0.12f, 14f), metalUrbano, t);
        MeshBox("Cable_Top_R", new Vector3(xCenter + 2.2f, 4.4f, zc), new Vector3(0.12f, 0.12f, 14f), metalUrbano, t);
        for (int i = 0; i <= 5; i++)
        {
            float z = zc - 6f + i * 2.4f;
            MeshCylinder("Cable_L", new Vector3(xCenter - 2.2f, 3.5f, z), 0.05f, 1.6f, metalUrbano, t, 8);
            MeshCylinder("Cable_R", new Vector3(xCenter + 2.2f, 3.5f, z), 0.05f, 1.6f, metalUrbano, t, 8);
        }
        MeshBox("Baranda_L", new Vector3(xCenter - w / 2f, 0.6f, zc), new Vector3(0.1f, 1.2f, 14f), metalUrbano, t);
        MeshBox("Baranda_R", new Vector3(xCenter + w / 2f, 0.6f, zc), new Vector3(0.1f, 1.2f, 14f), metalUrbano, t);
    }

    static void BuildBuildings(Transform t)
    {
        Skyscraper(t, "Rascacielos_Central", new Vector3(-33f, 0f, 97f), 18f, 18f, 62f);

        // Manzana NW (evita la plaza interactiva)
        Residential(t, "Res_NW1", new Vector3(-105f, 0f, 78f), 14f, 12f, 15f, 4);
        Residential(t, "Res_NW2", new Vector3(-85f, 0f, 78f), 12f, 12f, 11f, 3);
        Office(t, "Ofi_NW1", new Vector3(-110f, 0f, 108f), 18f, 14f, 22f, 6);
        Commercial(t, "Com_NW1", new Vector3(-95f, 0f, 70f), 16f, 10f, 8f);

        // Manzana N-centro oeste (rascacielos + medianos)
        Office(t, "Ofi_NC1", new Vector3(-18f, 0f, 82f), 15f, 12f, 18f, 5);
        Residential(t, "Res_NC1", new Vector3(-46f, 0f, 112f), 13f, 11f, 13f, 4);
        Commercial(t, "Com_NC1", new Vector3(-18f, 0f, 112f), 14f, 9f, 7f);

        // Manzana N-centro este
        Office(t, "Ofi_NC2", new Vector3(20f, 0f, 85f), 16f, 13f, 20f, 6);
        Residential(t, "Res_NC2", new Vector3(46f, 0f, 80f), 12f, 12f, 10f, 3);
        Residential(t, "Res_NC3", new Vector3(20f, 0f, 112f), 14f, 11f, 14f, 4);
        Office(t, "Ofi_NC3", new Vector3(46f, 0f, 112f), 15f, 12f, 17f, 5);

        // Manzana NE
        Residential(t, "Res_NE1", new Vector3(85f, 0f, 82f), 15f, 12f, 16f, 4);
        Residential(t, "Res_NE2", new Vector3(106f, 0f, 82f), 12f, 12f, 9f, 3);
        Office(t, "Ofi_NE1", new Vector3(106f, 0f, 110f), 17f, 13f, 24f, 7);
        Commercial(t, "Com_NE1", new Vector3(85f, 0f, 110f), 15f, 10f, 8f);

        // Manzana W-centro
        Office(t, "Ofi_WC1", new Vector3(-85f, 0f, 15f), 20f, 15f, 28f, 8);
        Office(t, "Ofi_WC2", new Vector3(-106f, 0f, 40f), 16f, 13f, 19f, 6);
        Residential(t, "Res_WC1", new Vector3(-85f, 0f, 44f), 13f, 11f, 12f, 3);
        Commercial(t, "Com_WC2", new Vector3(-106f, 0f, 15f), 14f, 9f, 7f);

        // Manzana E-centro
        Office(t, "Ofi_EC1", new Vector3(85f, 0f, 15f), 19f, 14f, 26f, 7);
        Office(t, "Ofi_EC2", new Vector3(106f, 0f, 40f), 15f, 12f, 18f, 5);
        Residential(t, "Res_EC1", new Vector3(85f, 0f, 44f), 13f, 11f, 12f, 3);
        Commercial(t, "Com_EC2", new Vector3(106f, 0f, 15f), 14f, 9f, 7f);

        // Manzana SW
        Residential(t, "Res_SW1", new Vector3(-85f, 0f, -15f), 16f, 13f, 17f, 5);
        Residential(t, "Res_SW2", new Vector3(-106f, 0f, -15f), 12f, 12f, 10f, 3);
        Office(t, "Ofi_SW1", new Vector3(-85f, 0f, -45f), 15f, 12f, 21f, 6);
        Commercial(t, "Com_SW1", new Vector3(-106f, 0f, -45f), 13f, 9f, 7f);

        // Manzana S-centro oeste
        Residential(t, "Res_SC1", new Vector3(-40f, 0f, -15f), 15f, 12f, 15f, 4);
        Residential(t, "Res_SC2", new Vector3(-18f, 0f, -45f), 13f, 11f, 12f, 3);
        Office(t, "Ofi_SC1", new Vector3(-40f, 0f, -45f), 16f, 13f, 20f, 6);
        Commercial(t, "Com_SC1", new Vector3(-18f, 0f, -15f), 13f, 9f, 7f);

        // Manzana S-centro este
        Residential(t, "Res_SC3", new Vector3(20f, 0f, -15f), 14f, 11f, 13f, 4);
        Residential(t, "Res_SC4", new Vector3(46f, 0f, -45f), 13f, 11f, 11f, 3);
        Office(t, "Ofi_SC2", new Vector3(46f, 0f, -15f), 16f, 13f, 19f, 5);
        Commercial(t, "Com_SC2", new Vector3(20f, 0f, -45f), 12f, 9f, 7f);

        // Manzana SE
        Residential(t, "Res_SE1", new Vector3(85f, 0f, -15f), 15f, 12f, 16f, 4);
        Office(t, "Ofi_SE1", new Vector3(106f, 0f, -15f), 16f, 13f, 22f, 6);
        Residential(t, "Res_SE2", new Vector3(85f, 0f, -45f), 12f, 12f, 10f, 3);
        Commercial(t, "Com_SE3", new Vector3(106f, 0f, -45f), 13f, 9f, 7f);

        // Frente al río
        Residential(t, "Res_RF1", new Vector3(-85f, 0f, -85f), 14f, 11f, 13f, 4);
        Residential(t, "Res_RF2", new Vector3(-106f, 0f, -85f), 12f, 12f, 9f, 3);
        Office(t, "Ofi_RF1", new Vector3(-33f, 0f, -85f), 15f, 12f, 18f, 5);
        Commercial(t, "Com_RF1", new Vector3(20f, 0f, -85f), 14f, 9f, 7f);
        Residential(t, "Res_RF3", new Vector3(85f, 0f, -85f), 14f, 11f, 14f, 4);
        Commercial(t, "Com_RF2", new Vector3(106f, 0f, -85f), 13f, 9f, 7f);
    }

    static void Skyscraper(Transform t, string name, Vector3 pos, float w, float d, float height)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        float tiers = 3f;
        for (int i = 0; i < tiers; i++)
        {
            float f = 1f - i * 0.16f;
            float tierH = height / tiers;
            float baseY = i * tierH;
            MeshBox("Nivel" + (i + 1), new Vector3(0f, baseY + tierH / 2f, 0f), new Vector3(w * f, tierH, d * f), concreto, p);
            for (int side = 0; side < 4; side++)
            {
                float gw = w * f - 2.4f;
                float gd = 0.08f;
                if (side == 0) MeshBox("Ventana", new Vector3(0f, baseY + tierH * 0.6f, d * f / 2f + 0.02f), new Vector3(gw, tierH * 0.45f, gd), vidrioEdificio, p);
                if (side == 1) MeshBox("Ventana", new Vector3(0f, baseY + tierH * 0.6f, -d * f / 2f - 0.02f), new Vector3(gw, tierH * 0.45f, gd), vidrioEdificio, p);
                if (side == 2) MeshBox("Ventana", new Vector3(w * f / 2f + 0.02f, baseY + tierH * 0.6f, 0f), new Vector3(gd, tierH * 0.45f, gd), vidrioEdificio, p);
                if (side == 3) MeshBox("Ventana", new Vector3(-w * f / 2f - 0.02f, baseY + tierH * 0.6f, 0f), new Vector3(gd, tierH * 0.45f, gd), vidrioEdificio, p);
            }
        }
        MeshBox("Corona", new Vector3(0f, height + 0.5f, 0f), new Vector3(w * 0.4f, 1f, d * 0.4f), metalUrbano, p);
    }

    static void Office(Transform t, string name, Vector3 pos, float w, float d, float height, int floors)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshBox("Cuerpo", new Vector3(0f, height / 2f, 0f), new Vector3(w, height, d), concreto, p);
        float fh = height / floors;
        for (int i = 0; i < floors; i++)
        {
            float y = i * fh + fh * 0.5f;
            MeshBox("VentanaF", new Vector3(0f, y, d / 2f + 0.02f), new Vector3(w - 1.2f, fh * 0.7f, 0.08f), vidrioEdificio, p);
            MeshBox("VentanaB", new Vector3(0f, y, -d / 2f - 0.02f), new Vector3(w - 1.2f, fh * 0.7f, 0.08f), vidrioEdificio, p);
            MeshBox("VentanaL", new Vector3(w / 2f + 0.02f, y, 0f), new Vector3(0.08f, fh * 0.7f, d - 1.2f), vidrioEdificio, p);
            MeshBox("VentanaR", new Vector3(-w / 2f - 0.02f, y, 0f), new Vector3(0.08f, fh * 0.7f, d - 1.2f), vidrioEdificio, p);
        }
        MeshBox("Techo", new Vector3(0f, height + 0.15f, 0f), new Vector3(w + 0.4f, 0.3f, d + 0.4f), concreto, p);
    }

    static void Residential(Transform t, string name, Vector3 pos, float w, float d, float height, int floors)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshBox("Cuerpo", new Vector3(0f, height / 2f, 0f), new Vector3(w, height, d), concreto, p);
        float fh = height / floors;
        for (int i = 0; i < floors; i++)
        {
            float y = i * fh + fh * 0.5f;
            MeshBox("VentanaF", new Vector3(0f, y, d / 2f + 0.02f), new Vector3(w - 1.6f, fh * 0.7f, 0.08f), vidrioEdificio, p);
            MeshBox("VentanaB", new Vector3(0f, y, -d / 2f - 0.02f), new Vector3(w - 1.6f, fh * 0.7f, 0.08f), vidrioEdificio, p);
            if (i % 2 == 0)
            {
                MeshBox("Balcon", new Vector3(-w / 2f - 1f, y, 0f), new Vector3(0.2f, 0.1f, d - 2f), concreto, p);
                MeshBox("Baranda", new Vector3(-w / 2f - 1.2f, y, 0f), new Vector3(0.08f, fh * 0.6f, d - 2f), metalUrbano, p);
            }
        }
        MeshBox("Techo", new Vector3(0f, height + 0.15f, 0f), new Vector3(w + 0.4f, 0.3f, d + 0.4f), concreto, p);
    }

    static void Commercial(Transform t, string name, Vector3 pos, float w, float d, float height)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshBox("Cuerpo", new Vector3(0f, height / 2f, 0f), new Vector3(w, height, d), concreto, p);
        MeshBox("VidrieraF", new Vector3(0f, 1.4f, d / 2f + 0.02f), new Vector3(w - 1.6f, 2.2f, 0.08f), vidrioEdificio, p);
        MeshBox("VidrieraB", new Vector3(0f, 1.4f, -d / 2f - 0.02f), new Vector3(w - 1.6f, 2.2f, 0.08f), vidrioEdificio, p);
        MeshBox("Marquesina", new Vector3(0f, 2.9f, d / 2f + 0.6f), new Vector3(w, 0.2f, 1.6f), metalUrbano, p);
    }

    static void BuildPark(Transform t)
    {
        // Plaza y caminos
        MeshBox("Plaza_Central", new Vector3(33.5f, 0.04f, 30f), new Vector3(30f, 0.12f, 30f), acera, t);
        MeshBox("Camino_NS", new Vector3(33.5f, 0.04f, 30f), new Vector3(6f, 0.12f, 60f), acera, t);
        MeshBox("Camino_EW", new Vector3(33.5f, 0.04f, 30f), new Vector3(60f, 0.12f, 6f), acera, t);

        // Fuente
        MeshCylinder("Fuente_Borde", new Vector3(33.5f, 0.4f, 30f), 4f, 0.8f, concreto, t, 24);
        MeshCylinder("Fuente_Agua", new Vector3(33.5f, 0.3f, 30f), 3.4f, 0.2f, agua, t, 24);
        MeshCylinder("Fuente_Pilar", new Vector3(33.5f, 1.4f, 30f), 0.5f, 2.4f, concreto, t, 12);
        MeshSphere("Fuente_Roca", new Vector3(33.5f, 2.8f, 30f), 0.5f, concreto, t);

        // Arboles del parque
        float[,] treePos = new float[,] {
            {15f, 45f}, {22f, 45f}, {45f, 45f}, {52f, 40f},
            {15f, 12f}, {25f, 12f}, {45f, 12f}, {52f, 18f},
            {20f, 30f}, {48f, 30f}, {33.5f, 12f}, {33.5f, 48f},
            {18f, 22f}, {50f, 22f}
        };
        for (int i = 0; i < treePos.GetLength(0); i++)
            Tree(t, "Arbol_Parque_" + (i + 1), new Vector3(treePos[i, 0], 0f, treePos[i, 1]));

        // Arboles en avenida (borde exterior de la vereda, evitando río y cruces)
        float[] treeZ = { -90f, -50f, -25f, 15f, 40f, 85f, 115f };
        for (int i = 0; i < treeZ.Length; i++)
        {
            Tree(t, "Arbol_Av_L_" + i, new Vector3(10f, 0f, treeZ[i]));
            Tree(t, "Arbol_Av_R_" + i, new Vector3(-10f, 0f, treeZ[i]));
        }

        // Bancas
        Bench(t, "Banca_Parque_1", new Vector3(24f, 0f, 22f), 0f);
        Bench(t, "Banca_Parque_2", new Vector3(43f, 0f, 22f), 0f);
        Bench(t, "Banca_Parque_3", new Vector3(24f, 0f, 38f), 180f);
        Bench(t, "Banca_Parque_4", new Vector3(43f, 0f, 38f), 180f);
    }

    static void Tree(Transform t, string name, Vector3 pos)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshCylinder("Tronco", new Vector3(0f, 1.4f, 0f), 0.4f, 2.8f, corteza, p, 12);
        MeshCylinder("Rama1", new Vector3(0.7f, 2.6f, 0f), 0.18f, 1.2f, corteza, p, 10);
        MeshCylinder("Rama2", new Vector3(-0.7f, 2.6f, 0.4f), 0.18f, 1.2f, corteza, p, 10);
        MeshSphere("Copa1", new Vector3(0f, 4.2f, 0f), 1.9f, follaje, p);
        MeshSphere("Copa2", new Vector3(0.9f, 3.6f, 0.4f), 1.3f, follaje, p);
        MeshSphere("Copa3", new Vector3(-0.9f, 3.6f, -0.4f), 1.3f, follaje, p);
    }

    static void Bench(Transform t, string name, Vector3 pos, float yaw)
    {
        GameObject go = NewGO(name, pos, t);
        go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Transform p = go.transform;
        MeshBox("Asiento", new Vector3(0f, 0.55f, 0f), new Vector3(2f, 0.1f, 0.5f), metalUrbano, p);
        MeshBox("Respaldo", new Vector3(0f, 1.0f, -0.25f), new Vector3(2f, 0.8f, 0.08f), metalUrbano, p);
        MeshBox("Pata_L", new Vector3(-0.8f, 0.25f, 0f), new Vector3(0.12f, 0.5f, 0.5f), metalUrbano, p);
        MeshBox("Pata_R", new Vector3(0.8f, 0.25f, 0f), new Vector3(0.12f, 0.5f, 0.5f), metalUrbano, p);
    }

    static void BuildVehicles(Transform t)
    {
        // Estacionados al borde de la avenida (sin bloquear la calzada)
        Car(t, "Sedan_Rojo", new Vector3(6f, 0.06f, 20f), 0f, carroRojo);
        Car(t, "Sedan_Azul", new Vector3(-6f, 0.06f, 32f), 180f, carroAzul);
        Car(t, "Sedan_Blanco", new Vector3(6f, 0.06f, 48f), 0f, carroBlanco);
        Car(t, "Sedan_Gris", new Vector3(-6f, 0.06f, 60f), 180f, metalVehiculo);
        Car(t, "Sedan_Amarelo", new Vector3(6f, 0.06f, -25f), 0f, carroAmarillo);
        Suv(t, "Suv_Azul", new Vector3(-6f, 0.06f, -40f), 180f, carroAzul);
        Suv(t, "Suv_Rojo", new Vector3(6f, 0.06f, 82f), 0f, carroRojo);
        Car(t, "Sedan_Azul2", new Vector3(-6f, 0.06f, 95f), 180f, carroAzul);
    }

    static void Car(Transform t, string name, Vector3 pos, float yaw, Material body)
    {
        GameObject go = NewGO(name, pos, t);
        go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Transform p = go.transform;
        MeshBox("Carroceria", new Vector3(0f, 0.55f, 0f), new Vector3(1.8f, 0.5f, 4.2f), body, p);
        MeshBox("Cabina", new Vector3(0f, 1.15f, -0.2f), new Vector3(1.6f, 0.7f, 2.4f), body, p);
        MeshBox("ParabrisasF", new Vector3(0f, 1.2f, 0.9f), new Vector3(1.5f, 0.6f, 0.06f), vidrio, p);
        MeshBox("ParabrisasB", new Vector3(0f, 1.2f, -1.3f), new Vector3(1.5f, 0.6f, 0.06f), vidrio, p);
        MeshBox("VentanaL", new Vector3(0.81f, 1.2f, -0.2f), new Vector3(0.06f, 0.6f, 1.8f), vidrio, p);
        MeshBox("VentanaR", new Vector3(-0.81f, 1.2f, -0.2f), new Vector3(0.06f, 0.6f, 1.8f), vidrio, p);
        MeshBox("Faro_L", new Vector3(0.55f, 0.6f, 2.1f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, p);
        MeshBox("Faro_R", new Vector3(-0.55f, 0.6f, 2.1f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, p);
        Wheel(p, "Rueda_FL", new Vector3(0.75f, 0.35f, 1.35f));
        Wheel(p, "Rueda_FR", new Vector3(-0.75f, 0.35f, 1.35f));
        Wheel(p, "Rueda_RL", new Vector3(0.75f, 0.35f, -1.35f));
        Wheel(p, "Rueda_RR", new Vector3(-0.75f, 0.35f, -1.35f));
    }

    static void Suv(Transform t, string name, Vector3 pos, float yaw, Material body)
    {
        GameObject go = NewGO(name, pos, t);
        go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Transform p = go.transform;
        MeshBox("Carroceria", new Vector3(0f, 0.7f, 0f), new Vector3(1.9f, 0.7f, 4.4f), body, p);
        MeshBox("Cabina", new Vector3(0f, 1.45f, -0.1f), new Vector3(1.7f, 0.8f, 2.8f), body, p);
        MeshBox("ParabrisasF", new Vector3(0f, 1.5f, 1.2f), new Vector3(1.6f, 0.7f, 0.06f), vidrio, p);
        MeshBox("ParabrisasB", new Vector3(0f, 1.5f, -1.5f), new Vector3(1.6f, 0.7f, 0.06f), vidrio, p);
        MeshBox("Faro_L", new Vector3(0.6f, 0.75f, 2.2f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, p);
        MeshBox("Faro_R", new Vector3(-0.6f, 0.75f, 2.2f), new Vector3(0.4f, 0.2f, 0.1f), carroAmarillo, p);
        Wheel(p, "Rueda_FL", new Vector3(0.8f, 0.45f, 1.4f), 0.45f);
        Wheel(p, "Rueda_FR", new Vector3(-0.8f, 0.45f, 1.4f), 0.45f);
        Wheel(p, "Rueda_RL", new Vector3(0.8f, 0.45f, -1.4f), 0.45f);
        Wheel(p, "Rueda_RR", new Vector3(-0.8f, 0.45f, -1.4f), 0.45f);
    }

    static void Bus(Transform t, string name, Vector3 pos, float yaw)
    {
        GameObject go = NewGO(name, pos, t);
        go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Transform p = go.transform;
        MeshBox("Cuerpo", new Vector3(0f, 1.4f, 0f), new Vector3(2.5f, 1.6f, 12f), carroAmarillo, p);
        MeshBox("Techo", new Vector3(0f, 2.6f, 0f), new Vector3(2.4f, 0.4f, 12f), carroAmarillo, p);
        MeshBox("VentanasL", new Vector3(1.26f, 1.9f, 0f), new Vector3(0.05f, 0.7f, 11f), vidrio, p);
        MeshBox("VentanasR", new Vector3(-1.26f, 1.9f, 0f), new Vector3(0.05f, 0.7f, 11f), vidrio, p);
        MeshBox("Parabrisas", new Vector3(0f, 1.8f, 6.0f), new Vector3(2.4f, 0.9f, 0.06f), vidrio, p);
        MeshBox("Faro_L", new Vector3(0.6f, 0.8f, 6.0f), new Vector3(0.4f, 0.3f, 0.1f), carroBlanco, p);
        MeshBox("Faro_R", new Vector3(-0.6f, 0.8f, 6.0f), new Vector3(0.4f, 0.3f, 0.1f), carroBlanco, p);
        Wheel(p, "Rueda_FL", new Vector3(1.05f, 0.55f, 4.2f), 0.55f);
        Wheel(p, "Rueda_FR", new Vector3(-1.05f, 0.55f, 4.2f), 0.55f);
        Wheel(p, "Rueda_RL", new Vector3(1.05f, 0.55f, -4.2f), 0.55f);
        Wheel(p, "Rueda_RR", new Vector3(-1.05f, 0.55f, -4.2f), 0.55f);
    }

    static void Wheel(Transform p, string name, Vector3 localPos, float radius = 0.35f)
    {
        MeshCylinder(name, localPos, radius, 0.3f, metalUrbano, p, 12);
        Transform wh = p.Find(name);
        if (wh != null) wh.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    static void BuildUrbanFurniture(Transform t)
    {
        // Postes de alumbrado a lo largo de la avenida
        for (int i = 0; i < 12; i++)
        {
            float z = -130f + i * 22f;
            Streetlight(t, "Farola_L_" + i, new Vector3(8.75f, 0f, z));
            Streetlight(t, "Farola_R_" + i, new Vector3(-8.75f, 0f, z));
        }
        // Paradas de autobus
        BusStop(t, "Parada_1", new Vector3(8.75f, 0f, -20f));
        BusStop(t, "Parada_2", new Vector3(-8.75f, 0f, 40f));
        // Hidrantes
        Hydrant(t, "Hidrante_1", new Vector3(30f, 0f, -30f));
        Hydrant(t, "Hidrante_2", new Vector3(-30f, 0f, 70f));
        Hydrant(t, "Hidrante_3", new Vector3(90f, 0f, 60f));
        // Muros perimetrales
        MeshBox("Muro_1", new Vector3(-110f, 1.0f, -110f), new Vector3(30f, 2f, 0.5f), concreto, t);
        MeshBox("Muro_2", new Vector3(-110f, 1.0f, -110f), new Vector3(0.5f, 2f, 20f), concreto, t);
        MeshBox("Muro_3", new Vector3(110f, 1.0f, 110f), new Vector3(30f, 2f, 0.5f), concreto, t);
    }

    static void Streetlight(Transform t, string name, Vector3 pos)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshCylinder("Poste", new Vector3(0f, 3.5f, 0f), 0.15f, 7f, metalUrbano, p, 10);
        MeshBox("Brazo", new Vector3(0.9f, 7f, 0f), new Vector3(1.8f, 0.12f, 0.12f), metalUrbano, p);
        MeshBox("Luminaria", new Vector3(1.8f, 6.9f, 0f), new Vector3(0.8f, 0.15f, 0.4f), emisivoFarola, p);
    }

    static void BusStop(Transform t, string name, Vector3 pos)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshBox("Techo", new Vector3(0f, 2.7f, 0f), new Vector3(1.6f, 0.15f, 5f), metalUrbano, p);
        MeshBox("Trasera", new Vector3(-0.7f, 1.4f, 0f), new Vector3(0.08f, 1.4f, 5f), vidrio, p);
        MeshBox("Pilar_F", new Vector3(0.7f, 1.4f, 2.2f), new Vector3(0.15f, 2.8f, 0.15f), metalUrbano, p);
        MeshBox("Pilar_B", new Vector3(0.7f, 1.4f, -2.2f), new Vector3(0.15f, 2.8f, 0.15f), metalUrbano, p);
        MeshBox("Banca", new Vector3(0.3f, 0.5f, -0.5f), new Vector3(0.5f, 0.1f, 2f), metalUrbano, p);
    }

    static void Hydrant(Transform t, string name, Vector3 pos)
    {
        GameObject go = NewGO(name, pos, t);
        Transform p = go.transform;
        MeshCylinder("Cuerpo", new Vector3(0f, 0.5f, 0f), 0.25f, 1f, carroRojo, p, 10);
        MeshCylinder("Capuchon", new Vector3(0f, 1.05f, 0f), 0.15f, 0.3f, carroRojo, p, 10);
        MeshBox("Valvula", new Vector3(0.4f, 0.7f, 0f), new Vector3(0.3f, 0.2f, 0.2f), carroRojo, p);
    }

    static void BuildLighting(Transform t)
    {
        GameObject sunGO = new GameObject("Sol_Direccional");
        sunGO.transform.SetParent(t, false);
        sunGO.transform.rotation = Quaternion.Euler(48f, -35f, 0f);
        Light sun = sunGO.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.color = new Color(1f, 0.96f, 0.88f);
        sun.intensity = 1.15f;
        sun.shadows = LightShadows.Soft;
        sun.shadowStrength = 0.9f;

        GameObject volumeGO = new GameObject("PostProcess_Global");
        volumeGO.transform.SetParent(t, false);
        Volume volume = volumeGO.AddComponent<Volume>();
        volume.isGlobal = true;
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        Tonemapping tonemap = profile.Add<Tonemapping>(true);
        tonemap.mode.value = TonemappingMode.ACES;
        Bloom bloom = profile.Add<Bloom>(true);
        bloom.intensity.value = 0.5f;
        bloom.threshold.value = 0.9f;
        bloom.scatter.value = 0.7f;
        volume.sharedProfile = profile;
        if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            AssetDatabase.CreateFolder("Assets", "Settings");
        AssetDatabase.CreateAsset(profile, "Assets/Settings/CityVolumeProfile.asset");

        // Ciclo día/noche
        GameObject cicloGO = new GameObject("Ciclo_Dia_Noche");
        cicloGO.transform.SetParent(t, false);
        cicloGO.AddComponent<DayNightCycle>();

        // Controlador de emisiones (ventanas + farolas)
        GameObject emissiveGO = new GameObject("Controlador_Emisivo");
        emissiveGO.transform.SetParent(t, false);
        NightEmissive ne = emissiveGO.AddComponent<NightEmissive>();
        SerializedObject neSo = new SerializedObject(ne);
        SerializedProperty matsProp = neSo.FindProperty("materials");
        matsProp.arraySize = 2;
        matsProp.GetArrayElementAtIndex(0).objectReferenceValue = vidrioEdificio;
        matsProp.GetArrayElementAtIndex(1).objectReferenceValue = emisivoFarola;
        neSo.ApplyModifiedPropertiesWithoutUndo();
    }

    static void BuildInteractivePlaza(Transform t)
    {
        GameObject plaza = NewGO("Plaza_Interactiva", new Vector3(0f, 0f, 0f), t);
        Transform p = plaza.transform;

        MeshBox("Pedestal_1", new Vector3(-80f, 0.8f, 100f), new Vector3(2f, 1.6f, 2f), concreto, p);
        MeshBox("Pedestal_2", new Vector3(-70f, 0.8f, 100f), new Vector3(2f, 1.6f, 2f), concreto, p);
        MeshBox("Pedestal_3", new Vector3(-75f, 0.8f, 108f), new Vector3(2f, 1.6f, 2f), concreto, p);

        TargetSphere(p, "Objetivo_1", new Vector3(-80f, 2.4f, 100f));
        TargetSphere(p, "Objetivo_2", new Vector3(-70f, 2.4f, 100f));
        TargetSphere(p, "Objetivo_3", new Vector3(-75f, 2.4f, 108f));

        Spawner(p, "Spawner_Cubo", new Vector3(-90f, 0.4f, 100f), "StandardCube");
        Spawner(p, "Spawner_Esfera", new Vector3(-90f, 0.4f, 108f), "BouncySphere");

        GameObject victory = NewGO("Zona_Victoria", new Vector3(-75f, 1f, 104f), p);
        BoxCollider vc = victory.AddComponent<BoxCollider>();
        vc.isTrigger = true;
        vc.size = new Vector3(24f, 4f, 24f);
        victory.AddComponent<VictoryZone>();
    }

    static void TargetSphere(Transform p, string name, Vector3 pos)
    {
        ProBuilderMesh pb = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, 0.8f, 1, true, false);
        pb.name = name;
        pb.transform.SetParent(p, false);
        pb.transform.localPosition = pos;
        pb.GetComponent<MeshRenderer>().sharedMaterial = carroRojo;
        pb.ToMesh();
        pb.Refresh();
        pb.gameObject.isStatic = false;
        SphereCollider sc = pb.gameObject.AddComponent<SphereCollider>();
        sc.radius = 0.8f;
        Rigidbody rb = pb.gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        pb.gameObject.AddComponent<TargetObjective>();
    }

    static void Spawner(Transform p, string name, Vector3 pos, string prefabName)
    {
        GameObject go = NewGO(name, pos, p);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Throwables/" + prefabName + ".prefab");
        ThrowableSpawner spawner = go.AddComponent<ThrowableSpawner>();
        if (prefab != null)
        {
            var so = new SerializedObject(spawner);
            so.FindProperty("throwablePrefab").objectReferenceValue = prefab;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    static void ImportPlayer(Scene cityScene, Transform folder)
    {
        GameObject rigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayerRig.prefab");
        if (rigPrefab == null)
        {
            Debug.LogWarning("CityBuilder: no se encontró Assets/Prefabs/PlayerRig.prefab, se omite el jugador.");
            return;
        }

        GameObject rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPrefab);
        rig.transform.SetParent(folder, false);
        rig.name = "PlayerRig";

        Transform player = rig.transform.Find("Player");
        if (player != null)
        {
            player.position = new Vector3(0f, 1.1f, -125f);
            player.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
