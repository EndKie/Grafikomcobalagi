using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class InstallLowPolyIslands
{
    private const string VisualName = "Low_Poly_Island_Visual";
    private const string ModelPath = "Assets/ThirdParty/LowPolyFloatingIsland/Island.fbx";
    private static readonly string[] Names = {
        "Math_Entry_Island", "Math_Finish_Island", "Math_High_Island",
        "Math_Upper_Arc_Island", "Final_Center_Island"
    };

    [MenuItem("Tools/Grafikom/Apply Low Poly Islands")]
    public static void Apply()
    {
        var scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlayingOrWillChangePlaymode ||
            scene.path != "Assets/Scenes/GetStarted_Scene.unity")
        {
            Debug.LogError("Open GetStarted_Scene in Edit Mode first.");
            return;
        }

        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        var meshes = model == null ? Array.Empty<MeshRenderer>() : model.GetComponentsInChildren<MeshRenderer>();
        var all = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Terrain>(true)).ToArray();
        var islands = Names.Select(name => all.Where(terrain => terrain.name == name).ToArray()).ToArray();
        if (meshes.Length != 1 || model.GetComponentInChildren<Collider>(true) != null ||
            islands.Any(matches => matches.Length != 1 || matches[0].terrainData == null ||
                !matches[0].TryGetComponent<TerrainCollider>(out var collider) ||
                !collider.enabled || collider.terrainData != matches[0].terrainData))
        {
            Debug.LogError("Expected a single island mesh without colliders and five valid target terrains.");
            return;
        }

        var bounds = meshes[0].bounds;
        var dimensions = bounds.size;
        if (!(dimensions.x > 0 && dimensions.y > 0 && dimensions.z > 0) ||
            float.IsInfinity(dimensions.sqrMagnitude))
        {
            Debug.LogError("Island FBX has invalid bounds.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Apply Low Poly Islands");
        foreach (var match in islands)
        {
            Terrain terrain = match[0];
            var old = terrain.transform.Find(VisualName);
            if (old != null)
                Undo.DestroyObjectImmediate(old.gameObject);

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, terrain.transform);
            Undo.RegisterCreatedObjectUndo(instance, "Place Island FBX");
            instance.name = VisualName;
            Vector3 size = terrain.terrainData.size;
            Vector3 scale = new(size.x / dimensions.x, size.y / dimensions.y, size.z / dimensions.z);
            // ponytail: top bounds align with the plateau; inspect rim and landing seams in Scene View.
            var sourceTop = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            var targetTop = new Vector3(size.x * 0.5f, terrain.terrainData.GetInterpolatedHeight(0.5f, 0.5f), size.z * 0.5f);
            instance.transform.localRotation = model.transform.rotation;
            instance.transform.localScale = Vector3.Scale(model.transform.localScale, scale);
            instance.transform.localPosition = targetTop + Vector3.Scale(model.transform.position - sourceTop, scale);
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);
            Undo.RecordObject(terrain, "Hide Terrain Visual");
            terrain.drawHeightmap = false;
            terrain.drawTreesAndFoliage = false;
        }
        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(scene);
    }
}
