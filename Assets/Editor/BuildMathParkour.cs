using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BuildMathParkour
{
    private const string ScenePath = "Assets/Scenes/GetStarted_Scene.unity";
    private const string BoxPath = "Assets/Starter Assets/Sample/Environment/Prefabs/Box_350x250x300_Prefab.prefab";
    private static readonly Vector3 GateOrigin = new(44f, 14f, 8f);
    private const int BuildVersion = 5;
    private const string RootName = "Math_Parkour_10_Questions";

    static BuildMathParkour()
    {
        EditorApplication.delayCall += BuildOnce;
    }

    private static void BuildOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // ponytail: scene kosong saat editor baru buka; ulang sampai scene aktif termuat.
        if (string.IsNullOrEmpty(SceneManager.GetActiveScene().path))
        {
            EditorApplication.delayCall += BuildOnce;
            return;
        }

        if (SceneManager.GetActiveScene().path != ScenePath)
            return;

        GameObject root = GameObject.Find(RootName);
        if (root == null || root.transform.Find($"Build_v{BuildVersion}") == null)
            Build();
    }

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

        Material pink = LoadMaterial("Material_Simple_Pink");
        Material purple = LoadMaterial("Material_Simple_Purple");
        Material blue = LoadMaterial("Material_Simple_BlueDark");
        Material blueLight = LoadMaterial("Material_Simple_BlueLight");
        Material aqua = LoadMaterial("Material_Simple_AquaMarine");
        Material orange = LoadMaterial("Material_Simple_Orange");
        Material yellow = LoadMaterial("Material_Simple_Yellow");
        Material green = LoadMaterial("Material_Simple_Green");
        if (pink == null || purple == null || blue == null || blueLight == null || aqua == null ||
            orange == null || yellow == null || green == null || LoadPrefab(BoxPath) == null)
        {
            return;
        }

        Transform normalRoot = new GameObject("Parkour_To_Math_Gate").transform;
        BuildNormalParkour(normalRoot, aqua, orange, yellow, green, blueLight);

        Transform mathRoot = new GameObject(RootName).transform;
        BuildGate(mathRoot, purple, pink, blue);
        BuildMathCourse(mathRoot, pink, purple, green, blueLight);
        AddKillZone(mathRoot);
        new GameObject($"Build_v{BuildVersion}").transform.SetParent(mathRoot);

        Debug.Assert(mathRoot.GetComponentsInChildren<MathQuestionStage>(true).Length == 10);
        Debug.Assert(mathRoot.GetComponentsInChildren<MathAnswerPlatform>(true).Length == 30);
        Debug.Assert(mathRoot.GetComponentsInChildren<MathCourseBoard>(true).Length == 1);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Selection.activeGameObject = mathRoot.gameObject;
        SceneView.lastActiveSceneView?.FrameSelected();
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

    private static void BuildGate(Transform root, Material purple, Material pink, Material blue)
    {
        CreateAssetBox("Gate_Turn_Floor", root, GateOrigin + new Vector3(0f, -0.35f, 3f),
            new Vector3(10f, 0.7f, 10f), blue);
        CreateAssetBox("Gate_Left", root, GateOrigin + new Vector3(0f, 2.8f, -5.2f),
            new Vector3(1f, 6.4f, 1f), purple);
        CreateAssetBox("Gate_Right", root, GateOrigin + new Vector3(0f, 2.8f, 5.2f),
            new Vector3(1f, 6.4f, 1f), purple);
        CreateAssetBox("Gate_Top", root, GateOrigin + new Vector3(0f, 5.7f, 0f),
            new Vector3(1f, 1f, 11.4f), purple);
        CreateAssetBox("Gate_Title_Board", root, GateOrigin + new Vector3(-0.1f, 3.8f, 0f),
            new Vector3(0.4f, 6f, 12f), pink, false);
        AddText("<b>MATH PARKOUR</b>\n10 SOAL ACAK\nBELOK KE KANAN →", root,
            GateOrigin + new Vector3(-0.35f, 3.8f, 0f), 2.6f, Color.white,
            new Vector2(11f, 6f), Quaternion.Euler(0f, 90f, 0f));
        AddCheckpoint(root, "Math_Gate_Checkpoint", GateOrigin + new Vector3(0f, 1.5f, 3f),
            new Vector3(8f, 3f, 7f));
    }

    private static void BuildMathCourse(Transform root, Material pink, Material purple, Material green,
        Material bridgeMaterial)
    {
        float firstRowZ = GateOrigin.z + 12f;
        const float spacing = 9f;
        CreateAssetBox("Math_Start_Bridge", root, new Vector3(GateOrigin.x, GateOrigin.y, GateOrigin.z + 7.5f),
            new Vector3(3.4f, 0.6f, 6f), bridgeMaterial);

        for (int stageIndex = 0; stageIndex < 10; stageIndex++)
        {
            float rowZ = firstRowZ + stageIndex * spacing;
            float rowY = GateOrigin.y + stageIndex * 0.3f;
            Transform stageRoot = new GameObject($"Question_{stageIndex + 1:00}").transform;
            stageRoot.SetParent(root);

            MathAnswerPlatform[] answers = new MathAnswerPlatform[3];
            for (int answerIndex = 0; answerIndex < 3; answerIndex++)
            {
                Vector3 position = new(GateOrigin.x + (answerIndex - 1) * 3.4f, rowY, rowZ);
                answers[answerIndex] = AddAnswer(stageRoot, position, stageIndex, answerIndex, pink, green);
            }

            float bridgeZ = rowZ + 5f;
            GameObject bridge = CreateAssetBox("Correct_Bridge", stageRoot,
                new Vector3(GateOrigin.x, rowY + 0.1f, bridgeZ), new Vector3(3.4f, 0.55f, 6f), green);
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

        // Satu papan soal besar di ujung lintasan, jadi terbaca dari semua baris.
        GameObject board = CreateAssetBox("Math_Question_Board", root,
            new Vector3(GateOrigin.x, GateOrigin.y + 9f, exitZ + 6f), new Vector3(26f, 12f, 0.6f), purple, false);
        TextMeshPro boardText = AddText("<b>SOAL 1/10</b>", board.transform, new Vector3(0f, 0f, -0.65f), 5.5f,
            Color.white, new Vector2(24f, 12f), Quaternion.identity, true);
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

        CreateAssetBox("Math_Exit", root, new Vector3(GateOrigin.x, GateOrigin.y + 3.6f, exitZ),
            new Vector3(9f, 0.8f, 5f), bridgeMaterial);
        AddCheckpoint(root, "Math_Finish", new Vector3(GateOrigin.x, GateOrigin.y + 5f, exitZ),
            new Vector3(9f, 3f, 5f));
        AddText("<b>MATH SELESAI</b>\nNEXT PARKOUR →", root,
            new Vector3(GateOrigin.x, GateOrigin.y + 6.4f, exitZ), 2.4f, Color.white,
            new Vector2(10f, 5f), Quaternion.identity);

        SpeedrunTimer timer = Object.FindFirstObjectByType<SpeedrunTimer>();
        if (timer != null)
        {
            SerializedObject serializedTimer = new(timer);
            serializedTimer.FindProperty("finishCheckpointName").stringValue = "Math_Finish";
            serializedTimer.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static MathAnswerPlatform AddAnswer(Transform parent, Vector3 position, int stageIndex,
        int answerIndex, Material pink, Material green)
    {
        GameObject platform = CreateAssetBox($"Answer_{stageIndex + 1:00}_{answerIndex + 1}", parent,
            position, new Vector3(3.2f, 0.7f, 3.2f), pink);
        Rigidbody body = platform.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;

        GameObject trigger = new("Step_Trigger", typeof(BoxCollider));
        trigger.transform.SetParent(platform.transform, false);
        trigger.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        BoxCollider triggerCollider = trigger.GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(0.95f, 1.5f, 0.95f);

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

    private static void AddKillZone(Transform root)
    {
        GameObject killZone = new("Math_Parkour_Respawn_Zone", typeof(BoxCollider));
        killZone.tag = "Water";
        killZone.transform.SetParent(root);
        killZone.transform.position = new Vector3(25f, 3f, 65f);
        killZone.transform.localScale = new Vector3(64f, 1f, 140f);
        killZone.GetComponent<BoxCollider>().isTrigger = true;
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
