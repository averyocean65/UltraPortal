using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/Build Asset Bundles")]
    public static void Build()
    {
        string outputDirectory = "Assets/Compiled Bundles";
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        BuildPipeline.BuildAssetBundles(outputDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
