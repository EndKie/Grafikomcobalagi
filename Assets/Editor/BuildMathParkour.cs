using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildMathParkour
{
    private const string ScenePath = "Assets/Scenes/GetStarted_Scene.unity";
    private const string BoxPath = "Assets/Starter Assets/Sample/Environment/Prefabs/Box_350x250x300_Prefab.prefab";
    private const string PlatformPath = "Assets/SourceFiles/Models/Platform.FBX";
    private const string EntryIslandTerrainPath = "Assets/Nature/Generated/MathEntryIslandTerrain.asset";
    private const string FinishIslandTerrainPath = "Assets/Nature/Generated/MathFinishIslandTerrain.asset";
    private const string HighIslandTerrainPath = "Assets/Nature/Generated/MathHighIslandTerrain.asset";
    private static readonly Vector3 GateOrigin = new(44f, 14f, 8f);
    private const int BuildVersion = 19;
    private const string RootName = "Math_Parkour_10_Questions";

    [MenuItem("Tools/Grafikom/Build Math Parkour _F8")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (SceneManager.GetActiveScene().path != ScenePath)
            EditorSceneManager.OpenScene(ScenePath);

        DestroyExisting("Parkour_To_Math_Gate");
        DestroyExisting("Math_Parkour_1");
        DestroyExisting(RootName);

        Material purple = LoadMaterial("Material_Simple_Purple");
        Material blue = LoadMaterial("Material_Simple_BlueDark");
        Material blueLight = LoadMaterial("Material_Simple_BlueLight");
        Material aqua = LoadMaterial("Material_Simple_AquaMarine");
        Material orange = LoadMaterial("Material_Simple_Orange");
        Material yellow = LoadMaterial("Material_Simple_Yellow");
        Material green = LoadMaterial("Material_Simple_Green");
        if (purple == null || blue == null || blueLight == null || aqua == null ||
            orange == null || yellow == null || green == null || LoadPrefab(BoxPath) == null ||
            LoadPrefab(PlatformPath) == null)
        {
            return;
        }

        Transform normalRoot = new GameObject("Parkour_To_Math_Gate").transform;
        BuildNormalParkour(normalRoot, aqua, orange, yellow, green, blueLight);

        Transform mathRoot = new GameObject(RootName).transform;
        BuildGate(mathRoot, purple, blue);
        BuildMathCourse(mathRoot, green);
        ConfigureWater();
        new GameObject($"Build_v{BuildVersion}").transform.SetParent(mathRoot);

        Debug.Assert(mathRoot.GetComponentsInChildren<MathQuestionStage>(true).Length == 10);
        Debug.Assert(mathRoot.GetComponentsInChildren<MathAnswerPlatform>(true).Length == 30);
        Debug.Assert(mathRoot.GetComponentsInChildren<MathCourseBoard>(true).Length == 1);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Selection.activeGameObject = null;
        Debug.Log("Parkour +X and 10-question random math parkour +Z built with project platform assets.");
    }

    private static void BuildNormalParkour(Transform root, Material aqua, Material orange, Material yellow,
        Material green, Material blueLight)
    {
        Vector3[] positions =
        {
            new(5f, 10.4f, 6.3f), new(8.6f, 10.75f, 8f), new(12.2f, 11.1f, 5.6f),
            new(15.8f, 11.45f, 7.8f), new(19.4f, 11.8f, 5.9f), new(23f, 12.15f, 8.2f),
            new(26.6f, 12.5f, 6f), new(30.2f, 12.85f, 8f), new(33.8f, 13.2f, 5.8f),
            new(37.4f, 13.55f, 7.6f), new(41f, 13.9f, 6.5f)
        };
        Material[] materials = { aqua, orange, yellow, green, blueLight };

        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 size = i is 0 or 10 ? new Vector3(3.8f, 0.7f, 3.8f) : new Vector3(3f, 0.65f, 3f);
            CreateAssetBox($"Parkour_X_{i + 1:00}", root, positions[i], size, materials[i % materials.Length]);
        }

        AddText("PARKOUR → MATH", root, new Vector3(23f, 15.5f, 7.4f), 2.2f, Color.white,
            new Vector2(12f, 3f), Quaternion.Euler(0f, 90f, 0f));
    }

    private static void BuildGate(Transform root, Material purple, Material blue)
    {
        CreateAssetBox("Gate_Turn_Floor", root, GateOrigin + new Vector3(0f, -0.35f, 3f),
            new Vector3(10f, 0.7f, 10f), blue);
        CreateAssetBox("Gate_Left", root, GateOrigin + new Vector3(0f, 2.8f, -5.2f),
            new Vector3(1f, 6.4f, 1f), purple);
        CreateAssetBox("Gate_Right", root, GateOrigin + new Vector3(0f, 2.8f, 5.2f),
            new Vector3(1f, 6.4f, 1f), purple);
        CreateAssetBox("Gate_Top", root, GateOrigin + new Vector3(0f, 5.7f, 0f),
            new Vector3(1f, 1f, 11.4f), purple);
        AddCheckpoint(root, "Math_Gate_Checkpoint", GateOrigin + new Vector3(0f, 1.5f, 3f),
            new Vector3(8f, 3f, 7f));
    }

    private static void BuildMathCourse(Transform root, Material green)
    {
        float firstRowZ = GateOrigin.z + 12f;
        const float spacing = 14f;
        const float rowRise = 0.55f;
        const float answerSpacing = 4.4f;
        CreateKitPlatform("Math_Start_Bridge", root,
            new Vector3(GateOrigin.x, GateOrigin.y, GateOrigin.z + 7.5f), new Vector3(3.4f, 0.6f, 6f));

        for (int stageIndex = 0; stageIndex < 10; stageIndex++)
        {
            float rowZ = firstRowZ + stageIndex * spacing;
            float rowY = GateOrigin.y + stageIndex * rowRise;
            Transform stageRoot = new GameObject($"Question_{stageIndex + 1:00}").transform;
            stageRoot.SetParent(root);

            MathAnswerPlatform[] answers = new MathAnswerPlatform[3];
            for (int answerIndex = 0; answerIndex < 3; answerIndex++)
            {
                Vector3 position = new(GateOrigin.x + (answerIndex - 1) * answerSpacing, rowY, rowZ);
                answers[answerIndex] = AddAnswer(stageRoot, position, stageIndex, answerIndex, green);
            }

            float bridgeZ = rowZ + spacing * 0.5f;
            GameObject bridge = CreateKitPlatform("Correct_Bridge", stageRoot,
                new Vector3(GateOrigin.x, rowY + 0.1f, bridgeZ), new Vector3(3.4f, 0.55f, 6f));
            AddCheckpoint(bridge.transform, $"Math_Checkpoint_{stageIndex + 1:00}",
                new Vector3(GateOrigin.x, rowY + 1.6f, bridgeZ + 1.8f), new Vector3(3f, 3f, 2f));
            bridge.SetActive(false);

            MathQuestionStage stage = stageRoot.gameObject.AddComponent<MathQuestionStage>();
            SerializedObject serialized = new(stage);
            serialized.FindProperty("stageNumber").intValue = stageIndex + 1;
            serialized.FindProperty("answerPlatforms").arraySize = answers.Length;
            for (int i = 0; i < answers.Length; i++)
                serialized.FindProperty("answerPlatforms").GetArrayElementAtIndex(i).objectReferenceValue = answers[i];
            serialized.FindProperty("correctBridge").objectReferenceValue = bridge;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        float exitZ = firstRowZ + 9 * spacing + 10f;
        float exitY = GateOrigin.y + 9 * rowRise + 0.9f;

        // Satu papan soal besar di ujung lintasan, jadi terbaca dari semua baris.
        GameObject board = CreatePlatformQuestionBoard(root,
            new Vector3(GateOrigin.x, exitY + 8.4f, exitZ + 6f), out float textZ);
        TextMeshPro boardText = AddText("<b>1+1=?</b>", board.transform, new Vector3(0f, 0f, textZ), 120f,
            Color.white, new Vector2(52f, 19f), Quaternion.identity, true);
        boardText.enableAutoSizing = true;
        boardText.fontSizeMin = 48f;
        boardText.fontSizeMax = 120f;
        MathCourseBoard courseBoard = board.AddComponent<MathCourseBoard>();
        SerializedObject serializedBoard = new(courseBoard);
        serializedBoard.FindProperty("label").objectReferenceValue = boardText;
        serializedBoard.ApplyModifiedPropertiesWithoutUndo();

        foreach (MathQuestionStage stage in root.GetComponentsInChildren<MathQuestionStage>(true))
        {
            SerializedObject serializedStage = new(stage);
            serializedStage.FindProperty("board").objectReferenceValue = courseBoard;
            serializedStage.ApplyModifiedPropertiesWithoutUndo();
        }

        CreateKitPlatform("Math_Exit", root, new Vector3(GateOrigin.x, exitY, exitZ),
            new Vector3(9f, 0.8f, 5f));
        AddCheckpoint(root, "Math_Finish", new Vector3(GateOrigin.x, exitY + 1.4f, exitZ),
            new Vector3(9f, 3f, 5f));
        AddText("<b>MATH SELESAI</b>\nNEXT PARKOUR →", root,
            new Vector3(GateOrigin.x, exitY + 2.8f, exitZ), 2.4f, Color.white,
            new Vector2(10f, 5f), Quaternion.identity);

        CreateMiniIsland(root, "Math_Entry_Island", EntryIslandTerrainPath,
            new Vector2(GateOrigin.x, GateOrigin.z + 2.5f), GateOrigin.y - 0.7f,
            new Vector3(18f, 11f, 15f), new Vector2(5.6f, 5.5f),
            new[]
            {
                new Vector2(37.8f, 11f), new Vector2(50.2f, 11f),
                new Vector2(44f, 17f), new Vector2(40.6f, 4.2f)
            },
            new[] { new Vector2(36.2f, 9f), new Vector2(51.8f, 12.6f) },
            new[] { new Vector2(38.4f, 14.5f), new Vector2(49.6f, 6.8f) });

        CreateMiniIsland(root, "Math_Finish_Island", FinishIslandTerrainPath,
            new Vector2(GateOrigin.x, exitZ + 4f), exitY - 0.55f,
            new Vector3(22f, 15f, 16f), new Vector2(7f, 6.5f),
            new[]
            {
                new Vector2(37f, 160f), new Vector2(51f, 160f),
                new Vector2(39f, 165f), new Vector2(49f, 165f)
            },
            new[] { new Vector2(35f, 160f), new Vector2(53f, 162f) },
            new[] { new Vector2(38f, 164f), new Vector2(50f, 163f) });

        CreateMiniIsland(root, "Math_High_Island", HighIslandTerrainPath,
            new Vector2(-45f, 160f), 39f,
            new Vector3(26f, 16f, 28f), new Vector2(8f, 9f),
            new[]
            {
                new Vector2(-53f, 157f), new Vector2(-37f, 158f),
                new Vector2(-52f, 166f), new Vector2(-38f, 168f),
                new Vector2(-45f, 171f), new Vector2(-35f, 164f)
            },
            new[] { new Vector2(-55f, 162f), new Vector2(-35f, 154f), new Vector2(-36f, 169f) },
            new[] { new Vector2(-52f, 168f), new Vector2(-37f, 166f), new Vector2(-54f, 155f) });

        SpeedrunTimer timer = Object.FindFirstObjectByType<SpeedrunTimer>();
        if (timer != null)
        {
            SerializedObject serializedTimer = new(timer);
            serializedTimer.FindProperty("finishCheckpointName").stringValue = "Math_Finish";
            serializedTimer.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static MathAnswerPlatform AddAnswer(Transform parent, Vector3 position, int stageIndex,
        int answerIndex, Material green)
    {
        GameObject platform = CreateKitPlatform($"Answer_{stageIndex + 1:00}_{answerIndex + 1}", parent,
            position, new Vector3(3.2f, 0.7f, 3.2f));
        Rigidbody body = platform.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;

        GameObject trigger = new("Step_Trigger", typeof(BoxCollider));
        trigger.transform.SetParent(platform.transform, false);
        trigger.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        BoxCollider triggerCollider = trigger.GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(3.2f, 1.5f, 3.2f);

        TextMeshPro label = AddText("?", platform.transform, new Vector3(0f, 0.1f, -1.75f), 2.4f,
            Color.white, new Vector2(3.2f, 3f), Quaternion.identity, true);
        MathAnswerPlatform answer = platform.AddComponent<MathAnswerPlatform>();
        SerializedObject serialized = new(answer);
        serialized.FindProperty("answerLabel").objectReferenceValue = label;
        serialized.FindProperty("correctMaterial").objectReferenceValue = green;
        serialized.FindProperty("visualRenderer").objectReferenceValue = platform.GetComponentInChildren<Renderer>();
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return answer;
    }

    private static void CreateMiniIsland(Transform parent, string name, string dataPath,
        Vector2 center, float plateauY, Vector3 size, Vector2 flatHalf,
        Vector2[] grassSpots, Vector2[] rockSpots, Vector2[] treeSpots)
    {
        TerrainLayer grass = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/ground_grass_fells_mossy/ground_grass_fells_mossy_CSNOH.terrainlayer");
        TerrainLayer rockyGrass = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/ground_rockgrass_fellsdirty/ground_rockgrass_fellsdirty_CSNOH.terrainlayer");
        TerrainLayer beach = AssetDatabase.LoadAssetAtPath<TerrainLayer>(
            "Assets/Nature/TerrainSample/aerial_beach/aerial_beach.terrainlayer");
        if (grass == null || rockyGrass == null || beach == null)
        {
            Debug.LogError("Terrain Sample layers missing. Mini island build skipped.");
            return;
        }

        TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(dataPath);
        if (data == null)
        {
            data = new TerrainData();
            AssetDatabase.CreateAsset(data, dataPath);
        }

        data.terrainLayers = new[] { grass, rockyGrass, beach };
        ConfigureMiniIslandData(data, size, flatHalf);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
        terrainObject.name = name;
        terrainObject.transform.SetParent(parent);
        terrainObject.transform.position = new Vector3(center.x - size.x * 0.5f,
            plateauY - size.y, center.y - size.z * 0.5f);

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.drawInstanced = true;
        terrain.heightmapPixelError = 3f;
        terrain.basemapDistance = 1000f;

        AddMiniIslandDecorations(terrain, terrainObject.transform, grassSpots, rockSpots, treeSpots);
    }

    private static float MiniIslandSlope01(float worldX, float worldZ, Vector3 size, Vector2 flatHalf)
    {
        float slopeX = Mathf.InverseLerp(flatHalf.x, size.x * 0.5f, Mathf.Abs(worldX));
        float slopeZ = Mathf.InverseLerp(flatHalf.y, size.z * 0.5f, Mathf.Abs(worldZ));
        return Mathf.SmoothStep(0f, 1f, Mathf.Max(slopeX, slopeZ));
    }

    private static void ConfigureMiniIslandData(TerrainData data, Vector3 size, Vector2 flatHalf)
    {
        const int resolution = 65;
        data.heightmapResolution = resolution;
        data.size = size;

        float[,] heights = new float[resolution, resolution];
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX = x / (resolution - 1f) * size.x - size.x * 0.5f;
                float worldZ = z / (resolution - 1f) * size.z - size.z * 0.5f;
                float slope = MiniIslandSlope01(worldX, worldZ, size, flatHalf);
                float noise = (Mathf.PerlinNoise((worldX + 100f) * 0.25f, (worldZ + 100f) * 0.25f) - 0.5f)
                              * 0.08f * slope;
                heights[z, x] = Mathf.Clamp01(1f - slope + noise);
            }
        }
        data.SetHeights(0, 0, heights);

        const int alphaResolution = 64;
        data.alphamapResolution = alphaResolution;
        float[,,] blend = new float[alphaResolution, alphaResolution, 3];
        for (int z = 0; z < alphaResolution; z++)
        {
            for (int x = 0; x < alphaResolution; x++)
            {
                float worldX = x / (alphaResolution - 1f) * size.x - size.x * 0.5f;
                float worldZ = z / (alphaResolution - 1f) * size.z - size.z * 0.5f;
                float slope = MiniIslandSlope01(worldX, worldZ, size, flatHalf);

                float beachWeight = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.3f, 0.95f, slope));
                float rockWeight = (1f - beachWeight) * Mathf.SmoothStep(0f, 1f,
                    Mathf.InverseLerp(0.4f, 0.8f, slope));
                float grassWeight = Mathf.Max(0f, 1f - beachWeight - rockWeight);
                float total = grassWeight + rockWeight + beachWeight;

                blend[z, x, 0] = grassWeight / total;
                blend[z, x, 1] = rockWeight / total;
                blend[z, x, 2] = beachWeight / total;
            }
        }
        data.SetAlphamaps(0, 0, blend);
        AssetDatabase.SaveAssets();
    }

    private static void AddMiniIslandDecorations(Terrain terrain, Transform parent,
        Vector2[] grassSpots, Vector2[] rockSpots, Vector2[] treeSpots)
    {
        GameObject grassPrefab = LoadPrefab("Assets/Nature/SplineSample/Prefabs/Grass01.prefab");
        GameObject rockPrefab = LoadPrefab("Assets/Nature/TerrainSample/Rocks/Rock_A_01.fbx");
        GameObject smallTreePrefab = LoadPrefab("Assets/Nature/SplineSample/Prefabs/TreeSmall.prefab");
        GameObject mediumTreePrefab = LoadPrefab("Assets/Nature/SplineSample/Prefabs/TreeMedium.prefab");

        for (int i = 0; i < grassSpots.Length; i++)
            PlaceMiniDecoration(grassPrefab, terrain, parent, grassSpots[i], 0.7f, 37f * i, $"Grass_{i + 1:00}");
        for (int i = 0; i < rockSpots.Length; i++)
            PlaceMiniDecoration(rockPrefab, terrain, parent, rockSpots[i], 0.5f, 63f * i + 20f, $"Rock_{i + 1:00}");
        for (int i = 0; i < treeSpots.Length; i++)
            PlaceMiniDecoration(i % 2 == 0 ? smallTreePrefab : mediumTreePrefab,
                terrain, parent, treeSpots[i], 0.75f, 71f * i + 15f, $"Tree_{i + 1:00}");
    }

    private static void PlaceMiniDecoration(GameObject prefab, Terrain terrain, Transform parent,
        Vector2 spot, float scale, float yaw, string name)
    {
        if (prefab == null)
            return;

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.SetParent(parent);
        instance.transform.position = new Vector3(spot.x,
            terrain.SampleHeight(new Vector3(spot.x, 0f, spot.y)) + terrain.transform.position.y, spot.y);
        instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        instance.transform.localScale *= scale;
    }

    private static void ConfigureWater()
    {
        GameObject water = GameObject.Find("Water");
        if (water == null)
        {
            Debug.LogError("Missing scene Water object.");
            return;
        }

        water.transform.position = new Vector3(25f, -1.5f, 72f);
        water.transform.localScale = new Vector3(200f, 1f, 240f);
    }

    private static GameObject CreateKitPlatform(string name, Transform parent, Vector3 position, Vector3 size)
    {
        GameObject root = new(name);
        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefab(PlatformPath), root.transform);
        visual.name = "Platform_Kit_Visual";
        visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
        visual.transform.localScale = Vector3.one;
        foreach (Collider collider in visual.GetComponentsInChildren<Collider>())
            collider.enabled = false;

        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);
        visual.transform.localScale = new Vector3(size.x / bounds.size.x, size.z / bounds.size.z,
            size.y / bounds.size.y);

        bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);
        visual.transform.position += position - bounds.center;

        BoxCollider rootCollider = root.AddComponent<BoxCollider>();
        rootCollider.size = size;
        return root;
    }

    private static GameObject CreatePlatformQuestionBoard(Transform parent, Vector3 position, out float textZ)
    {
        GameObject root = new("Math_Question_Board");
        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefab(PlatformPath), root.transform);
        visual.name = "Platform_Kit_Board_Visual";
        visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
        visual.transform.localScale = Vector3.one;
        foreach (Collider collider in visual.GetComponentsInChildren<Collider>())
            collider.enabled = false;

        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);
        visual.transform.localScale = new Vector3(56f / bounds.size.x, 22f / bounds.size.y,
            1f / bounds.size.z);

        bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);
        visual.transform.position += position - bounds.center;

        bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);
        textZ = root.transform.InverseTransformPoint(new Vector3(position.x, position.y, bounds.min.z - 0.15f)).z;
        return root;
    }

    private static GameObject CreateAssetBox(string name, Transform parent, Vector3 position, Vector3 size,
        Material material, bool addCollider = true)
    {
        GameObject root = new(name);
        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefab(BoxPath), root.transform);
        visual.name = "Platform_Asset";
        visual.transform.localPosition = new Vector3(0f, 0f, -size.z * 0.5f);
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = new Vector3(size.x / 3.5f, size.y / 2.5f, size.z / 3f);
        foreach (Collider collider in visual.GetComponentsInChildren<Collider>())
            collider.enabled = false;
        foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>())
            renderer.sharedMaterial = material;

        if (addCollider)
        {
            BoxCollider collider = root.AddComponent<BoxCollider>();
            collider.size = size;
        }
        return root;
    }

    private static void AddCheckpoint(Transform parent, string name, Vector3 position, Vector3 size)
    {
        GameObject checkpoint = new(name, typeof(BoxCollider));
        checkpoint.tag = "Checkpoint";
        checkpoint.transform.SetParent(parent);
        checkpoint.transform.position = position;
        BoxCollider collider = checkpoint.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = size;
    }

    private static TextMeshPro AddText(string content, Transform parent, Vector3 position, float size, Color color,
        Vector2 bounds, Quaternion rotation, bool local = false)
    {
        GameObject textObject = new("Label", typeof(TextMeshPro));
        textObject.transform.SetParent(parent, false);
        if (local)
        {
            textObject.transform.localPosition = position;
            textObject.transform.localRotation = rotation;
        }
        else
        {
            textObject.transform.position = position;
            textObject.transform.rotation = rotation;
        }

        TextMeshPro text = textObject.GetComponent<TextMeshPro>();
        text.text = content;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = size;
        text.color = color;
        text.rectTransform.sizeDelta = bounds;
        return text;
    }

    private static Material LoadMaterial(string name)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/{name}.mat");
        if (material == null)
            Debug.LogError("Missing math parkour material: " + name);
        return material;
    }

    private static GameObject LoadPrefab(string path)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
            Debug.LogError("Missing parkour asset: " + path);
        return prefab;
    }

    private static void DestroyExisting(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing);
    }
}
