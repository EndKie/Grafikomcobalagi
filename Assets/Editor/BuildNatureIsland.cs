using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BuildNatureIsland
{
    private const string ScenePath = "Assets/Scenes/GetStarted_Scene.unity";
    private const string TerrainDataPath = "Assets/Nature/Generated/IslandTerrain.asset";

    static BuildNatureIsland()
    {
        EditorApplication.delayCall += BuildOnce;
    }

    private static void BuildOnce()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
            SceneManager.GetActiveScene().path == ScenePath &&
            (GameObject.Find("Nature_Island") == null ||
             GameObject.Find("Rock_01") == null ||
             GameObject.Find("Speedrun_Leaderboard_Board") == null ||
             GameObject.Find("Board_Main0") == null))
        {
            Build();
        }
    }

    [MenuItem("Tools/Grafikom/Build Nature Island")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        EnsureFolder("Assets/Nature/Generated");

        TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(TerrainDataPath);
        if (terrainData == null)
        {
            terrainData = new TerrainData();
            AssetDatabase.CreateAsset(terrainData, TerrainDataPath);
        }

        ConfigureTerrainData(terrainData);

        GameObject existing = GameObject.Find("Nature_Island");
        if (existing != null)
            Object.DestroyImmediate(existing);

        GameObject existingDecoration = GameObject.Find("Nature_Decoration");
        if (existingDecoration != null)
            Object.DestroyImmediate(existingDecoration);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "Nature_Island";
        terrainObject.transform.position = new Vector3(-30f, -2f, -30f);

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.drawInstanced = true;
        terrain.heightmapPixelError = 3f;
        terrain.basemapDistance = 1000f;

        GameObject oldGround = GameObject.Find("Grass_Island");
        if (oldGround != null)
        {
            oldGround.name = "Legacy_Grass_Island";
            oldGround.SetActive(false);
        }

        Transform nature = new GameObject("Nature_Decoration").transform;
        AddTrees(terrain, nature);
        AddGrass(terrain, nature);
        AddRocks(terrain, nature);
        TMP_Text boardText = AddLeaderboard(terrain, nature);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            SpeedrunTimer timer = player.GetComponent<SpeedrunTimer>();
            if (timer == null)
                timer = player.AddComponent<SpeedrunTimer>();

            SerializedObject serializedTimer = new SerializedObject(timer);
            serializedTimer.FindProperty("leaderboardText").objectReferenceValue = boardText;
            serializedTimer.FindProperty("finishCheckpointName").stringValue = "Checkpoint_3";
            serializedTimer.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("Nature island, vegetation, and local speedrun leaderboard built.");
    }

    private static void ConfigureTerrainData(TerrainData terrainData)
    {
        const int resolution = 129;
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(60f, 10f, 60f);

        float[,] heights = new float[resolution, resolution];
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX = x / (resolution - 1f) * 60f - 30f;
                float worldZ = z / (resolution - 1f) * 60f - 30f;
                heights[z, x] = (IslandHeight(worldX, worldZ) + 2f) / 10f;
            }
        }
        terrainData.SetHeights(0, 0, heights);

        TerrainLayer grass = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/ground_grass_fells_mossy/ground_grass_fells_mossy_CSNOH.terrainlayer");
        TerrainLayer rockyGrass = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/ground_rockgrass_fellsdirty/ground_rockgrass_fellsdirty_CSNOH.terrainlayer");
        TerrainLayer beach = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/aerial_beach/aerial_beach.terrainlayer");

        if (grass == null || rockyGrass == null || beach == null)
        {
            Debug.LogError("Terrain Sample layers missing. Nature island build stopped.");
            return;
        }

        terrainData.terrainLayers = new[] { grass, rockyGrass, beach };
        terrainData.alphamapResolution = 64;

        float[,,] blend = new float[64, 64, 3];
        for (int z = 0; z < 64; z++)
        {
            for (int x = 0; x < 64; x++)
            {
                float worldX = x / 63f * 60f - 30f;
                float worldZ = z / 63f * 60f - 30f;
                float radius = new Vector2(worldX, worldZ).magnitude;
                float beachWeight = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(22f, 29f, radius));
                float rockWeight = (1f - beachWeight) * Mathf.SmoothStep(0f, 1f,
                    Mathf.InverseLerp(5.3f, 7f, IslandHeight(worldX, worldZ)));
                float grassWeight = Mathf.Max(0f, 1f - beachWeight - rockWeight);
                float total = grassWeight + rockWeight + beachWeight;

                blend[z, x, 0] = grassWeight / total;
                blend[z, x, 1] = rockWeight / total;
                blend[z, x, 2] = beachWeight / total;
            }
        }
        terrainData.SetAlphamaps(0, 0, blend);
    }

    private static float IslandHeight(float x, float z)
    {
        float radius = new Vector2(x, z).magnitude;
        float safeArea = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(11f, 17f, radius));
        float noise = (Mathf.PerlinNoise((x + 40f) * 0.09f, (z + 40f) * 0.09f) - 0.5f) * 0.7f * safeArea;
        float hills = Hill(x, z, -17f, -12f, 2.2f, 8f) +
                      Hill(x, z, 17f, 15f, 1.8f, 7f) +
                      Hill(x, z, -16f, 16f, 1.4f, 6f);
        float edgeDrop = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(23f, 30f, radius)) * 7f;
        return Mathf.Clamp(4.98f + noise + hills - edgeDrop, -2f, 7.5f);
    }

    private static float Hill(float x, float z, float centerX, float centerZ, float height, float radius)
    {
        float distance = Vector2.Distance(new Vector2(x, z), new Vector2(centerX, centerZ));
        float t = Mathf.Clamp01(1f - distance / radius);
        return height * t * t * (3f - 2f * t);
    }

    private static void AddTrees(Terrain terrain, Transform parent)
    {
        string big = "Assets/Nature/SplineSample/Prefabs/TreeBig.prefab";
        string medium = "Assets/Nature/SplineSample/Prefabs/TreeMedium.prefab";
        string small = "Assets/Nature/SplineSample/Prefabs/TreeSmall.prefab";

        Spawn(big, terrain, parent, new Vector2(-17f, -12f), 1.25f, 18f);
        Spawn(big, terrain, parent, new Vector2(17f, 15f), 1.1f, -35f);
        Spawn(medium, terrain, parent, new Vector2(-16f, 16f), 1.3f, 64f);
        Spawn(medium, terrain, parent, new Vector2(20f, -12f), 1.15f, 120f);
        Spawn(small, terrain, parent, new Vector2(-21f, 2f), 1.2f, -70f);
        Spawn(small, terrain, parent, new Vector2(2f, 21f), 1.1f, 42f);
    }

    private static void AddGrass(Terrain terrain, Transform parent)
    {
        string grass = "Assets/Nature/SplineSample/Prefabs/Grass01.prefab";
        Vector2[] positions =
        {
            new(-12f, -16f), new(-15f, -8f), new(13f, 16f), new(19f, 10f),
            new(-19f, 13f), new(4f, 19f), new(18f, -9f), new(-21f, -2f)
        };

        for (int i = 0; i < positions.Length; i++)
            Spawn(grass, terrain, parent, positions[i], 0.65f + i % 3 * 0.12f, i * 37f);
    }

    private static void AddRocks(Terrain terrain, Transform parent)
    {
        const string rock = "Assets/Nature/TerrainSample/Rocks/Rock_A_01.fbx";
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Material_SandLumpy.mat");
        Vector2[] positions =
        {
            new(-19f, -16f), new(21f, 4f), new(-8f, 22f), new(16f, -17f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject instance = Spawn(rock, terrain, parent, positions[i], 0.45f + i * 0.08f, 23f + i * 71f);
            if (instance == null)
                continue;

            instance.name = $"Rock_{i + 1:00}";
            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>())
                renderer.sharedMaterial = material;
        }
    }

    private static GameObject Spawn(string path, Terrain terrain, Transform parent, Vector2 position, float scale, float yaw)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Missing nature asset: " + path);
            return null;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent);
        instance.transform.position = new Vector3(position.x, TerrainY(terrain, position.x, position.y), position.y);
        instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        instance.transform.localScale = Vector3.Scale(instance.transform.localScale, Vector3.one * scale);
        return instance;
    }

    private static TMP_Text AddLeaderboard(Terrain terrain, Transform parent)
    {
        GameObject boardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Nature/WoodenBoard/Prefabs/WoodenBoard.prefab");
        if (boardPrefab == null)
        {
            Debug.LogError("Missing WoodenBoard prefab.");
            return null;
        }

        GameObject board = (GameObject)PrefabUtility.InstantiatePrefab(boardPrefab);
        board.name = "Speedrun_Leaderboard_Board";
        board.transform.SetParent(parent);
        board.transform.position = new Vector3(13f, TerrainY(terrain, 13f, 3f), 3f);
        board.transform.rotation = Quaternion.Euler(0f, 225f, 0f);
        board.transform.localScale = Vector3.one * 0.85f;

        GameObject textObject = new GameObject("Leaderboard_Text", typeof(TextMeshPro));
        textObject.transform.SetParent(board.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 2.45f, -0.16f);
        textObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        textObject.transform.localScale = Vector3.one * 0.7f;

        TextMeshPro text = textObject.GetComponent<TextMeshPro>();
        text.text = "<b>LEADERBOARD</b>\nWAKTU: 00:00.00\nTERBAIK: --:--.--";
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 1.6f;
        text.color = new Color(1f, 0.9f, 0.55f);
        text.rectTransform.sizeDelta = new Vector2(6.5f, 2.8f);
        return text;
    }

    private static float TerrainY(Terrain terrain, float x, float z)
    {
        return terrain.SampleHeight(new Vector3(x, 0f, z)) + terrain.transform.position.y;
    }

    private static void EnsureFolder(string path)
    {
        string[] parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
