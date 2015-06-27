using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveProcess : UnityEditor.AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        Debug.Log("wutasdaasd");
        string sceneName;
        sceneName = "";

        foreach (string path in paths)
            if (path.Contains(".unity"))
                sceneName = Path.GetFileNameWithoutExtension(path);

        if (sceneName.Length == 0)
            return paths;

        Debug.Log("wut");
        foreach (BezierPath bezierPath in GameObject.FindObjectsOfType<BezierPath>())
            bezierPath.SavePositions();

        return paths;
    }
}
