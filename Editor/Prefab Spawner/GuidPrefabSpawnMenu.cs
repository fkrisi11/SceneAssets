#if UNITY_EDITOR
using UnityEditor;

public static class GuidPrefabSpawnMenu
{
    private const string PostProcessingCamera = "0f84232749f14e2439dfd86a4af131fd";
    private const string VRCMirror = "c9903f281e5d7e246b7f06d26a0d4395";

    [MenuItem("TohruTheDragon/Scene Assets/Spawn Post Processing Camera")]
    public static void SpawnCamera()
        => GuidPrefabSpawner.SpawnPrefab(PostProcessingCamera,
                                         spawnRelativeToSelection: true,
                                         distance: 1f,
                                         lookAtTarget: true,
                                         invertLook: false,
                                         unpackPrefab: true);

    [MenuItem("TohruTheDragon/Scene Assets/Spawn Mirror")]
    public static void SpawnMirror()
        => GuidPrefabSpawner.SpawnPrefab(VRCMirror,
                                         spawnRelativeToSelection: true,
                                         distance: 1f,
                                         lookAtTarget: true,
                                         invertLook: true,
                                         unpackPrefab: true);
}
#endif
