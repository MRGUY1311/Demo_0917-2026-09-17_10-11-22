using UnityEditor;

public static class ReserializePolygonDungeon
{
    [MenuItem("Tools/Reserialize PolygonDungeon")]
    private static void Run()
    {
        var paths = new System.Collections.Generic.List<string>();

        foreach (var guid in AssetDatabase.FindAssets("", new[] { "Assets/PolygonDungeon" }))
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));

        AssetDatabase.ForceReserializeAssets(
            paths,
            ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
    }
}
