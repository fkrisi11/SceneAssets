#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GuidPrefabSpawner
{
    public static GameObject SpawnPrefab(string guid, bool spawnRelativeToSelection = true, float distance = 1, bool lookAtTarget = true, bool invertLook = false, bool unpackPrefab = false)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError("[GuidPrefabSpawner] GUID is null/empty.");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"[GuidPrefabSpawner] No asset found for GUID: {guid}");
            return null;
        }

        Object asset = AssetDatabase.LoadMainAssetAtPath(path);
        if (asset == null)
        {
            Debug.LogError($"[GuidPrefabSpawner] Failed to load asset at path: {path}");
            return null;
        }

        if (asset is not GameObject prefabAsset)
        {
            Debug.LogError($"[GuidPrefabSpawner] Asset is not a GameObject prefab: {asset.GetType().Name} ({path})");
            return null;
        }

        Object instance = PrefabUtility.InstantiatePrefab(prefabAsset, EditorSceneManager.GetActiveScene());
        GameObject go = instance as GameObject;

        if (go == null)
        {
            Debug.LogError($"[GuidPrefabSpawner] Instantiation failed for prefab: {path}");
            return null;
        }

        Undo.RegisterCreatedObjectUndo(go, "Spawn Prefab By GUID");

        if (unpackPrefab && PrefabUtility.IsAnyPrefabInstanceRoot(go))
        {
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }

        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;

        if (spawnRelativeToSelection)
        {
            Transform selected = Selection.activeTransform;
            if (selected != null)
            {
                Vector3 targetPos = selected.position;

                Vector3 spawnPos = targetPos + (selected.forward.normalized * distance);

                go.transform.position = spawnPos;

                if (lookAtTarget)
                {
                    if (!invertLook)
                    {
                        Vector3 toTarget = targetPos - spawnPos;
                        if (toTarget.sqrMagnitude > 0.000001f)
                        {
                            go.transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                        }
                    }
                    else
                    {
                        Vector3 awayFromTarget = spawnPos - targetPos;
                        if (awayFromTarget.sqrMagnitude > 0.000001f)
                        {
                            go.transform.rotation = Quaternion.LookRotation(awayFromTarget.normalized, Vector3.up);
                        }
                    }
                }
            }
        }

        Selection.activeGameObject = go;
        EditorGUIUtility.PingObject(go);

        Debug.Log($"[GuidPrefabSpawner] Spawned {go.name}");
        return go;
    }
}
#endif
