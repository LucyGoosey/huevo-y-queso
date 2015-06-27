using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    MonoScript script;
    BezierPath path;

    void OnEnable()
    {
        path = (BezierPath)target;
        script = MonoScript.FromMonoBehaviour(path);
    }
    public override void OnInspectorGUI()
    {
        script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);

        if (GUILayout.Button("Generate path!"))
            ((BezierPath)target).GeneratePath();
        if (GUILayout.Button("Open Editor"))
            EditorWindow.GetWindow<BezierPathWindow>();
    }
}