using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshFilterChecker : EditorWindow
{
    [MenuItem("Tools/Check Missing Meshes")]
    public static void CheckMissingMeshes()
    {
        // Find all MeshFilter components in the scene
        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();

        // Dictionary to group GameObjects by their prefab source
        Dictionary<GameObject, List<GameObject>> prefabGroups = new Dictionary<GameObject, List<GameObject>>();
        List<GameObject> nonPrefabObjects = new List<GameObject>();

        int missingMeshCount = 0;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // Check if the mesh is null
            if (meshFilter.sharedMesh == null)
            {
                missingMeshCount++;
                GameObject obj = meshFilter.gameObject;

                // Get the prefab source
                GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefabSource != null)
                {
                    // This is a prefab instance
                    if (!prefabGroups.ContainsKey(prefabSource))
                    {
                        prefabGroups[prefabSource] = new List<GameObject>();
                    }
                    prefabGroups[prefabSource].Add(obj);
                }
                else
                {
                    // This is not a prefab instance
                    nonPrefabObjects.Add(obj);
                }
            }
        }

        if (missingMeshCount == 0)
        {
            Debug.Log("All MeshFilters have meshes assigned. No issues found!");
            return;
        }

        Debug.LogWarning($"Found {missingMeshCount} MeshFilter(s) with missing meshes:\n");

        // Log prefab groups
        foreach (var kvp in prefabGroups.OrderByDescending(x => x.Value.Count))
        {
            GameObject prefab = kvp.Key;
            List<GameObject> instances = kvp.Value;

            Debug.LogWarning($"Prefab: '{prefab.name}' - {instances.Count} instance(s) with missing mesh", prefab);

            // Log each instance with clickable reference
            foreach (GameObject instance in instances)
            {
                Debug.LogWarning($"  └─ Instance: '{instance.name}'", instance);
            }
        }

        // Log non-prefab objects
        if (nonPrefabObjects.Count > 0)
        {
            Debug.LogWarning($"\nNon-prefab GameObjects with missing meshes: {nonPrefabObjects.Count}");
            foreach (GameObject obj in nonPrefabObjects)
            {
                Debug.LogWarning($"  └─ '{obj.name}' (not a prefab instance)", obj);
            }
        }
    }
}