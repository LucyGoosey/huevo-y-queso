using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    MonoScript script;
    BezierPath bPath;

    void OnEnable()
    {
        bPath = (BezierPath)target;
        script = MonoScript.FromMonoBehaviour(bPath);
    }

    public override void OnInspectorGUI()
    {
        script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);

        if (GUILayout.Button("Clear Path"))
            bPath.ClearPath();
        if (GUILayout.Button("Generate random path!"))
            bPath.RandomPath();
        if (GUILayout.Button("Generate handles"))
            bPath.CreatePathHandles();
        if (GUILayout.Button("Open Editor"))
            OpenEditorWindow();
    }

    private void OpenEditorWindow()
    {
        BezierAnchor last = null;
        uint count = 0;

        if (bPath.StartPoint != null)
        {
            last = bPath.StartPoint;
            ++count;

            while (last.next != null)
            {
                ++count;
                last = last.next;
            }
        }

        BezierPathWindow win = EditorWindow.GetWindow<BezierPathWindow>(string.Format("Editing: {0}", bPath.name));
        win.Setup(bPath, count, last);
    }
}