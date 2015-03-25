using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif

public class BezierPathCreator : EditorWindow {

    public static bool bDrawHandleLines = true;

    public bool bIsPlacing = false;

    private BezierPoint lastPoint = null;
    private BezierPoint curPoint = null;

    [MenuItem("Window/Bézier Path Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<BezierPathCreator>();
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnGUI()
    {
        bIsPlacing = GUI.Toggle(new Rect(0f, 0f, 60f, 30f), bIsPlacing, "Placing", "Button");

        if (GUI.Button(new Rect(0f, 35f, 60f, 30f), "Link"))
            LinkToStart();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (bIsPlacing)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        else
            return;

        Event cur = Event.current;

        if (cur.button != 0)
            return;

        switch(cur.type)
        {
            case EventType.MouseDown:
                GameObject gObject = new GameObject("BézierPoint", typeof(BezierPoint));
                gObject.transform.position = GetClickPos(cur.mousePosition, sceneView);

                curPoint = gObject.GetComponent<BezierPoint>();
                AddHandles(curPoint);

                cur.Use();
                break;

            case EventType.MouseDrag:
                curPoint.transform.position = GetClickPos(cur.mousePosition, sceneView);

                DestroyImmediate(curPoint.handleA.gameObject);
                DestroyImmediate(lastPoint.handleB.gameObject);

                AddHandles(curPoint);
                break;

            case EventType.MouseUp:
                lastPoint = curPoint;
                curPoint = null;
                break;

            default:
                break;
        }
    }

    Vector3 GetClickPos(Vector2 mousePos, SceneView sceneView)
    {
        mousePos.y = sceneView.camera.pixelHeight - mousePos.y;

        Vector3 worldPos = sceneView.camera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        return worldPos;
    }

    void AddHandles(BezierPoint _bPoint)
    {
        GameObject newA = new GameObject("BézierHandle A", typeof(BezierHandle));
        newA.transform.position = _bPoint.transform.position;
        newA.transform.parent = _bPoint.transform;

        BezierHandle handA = newA.GetComponent<BezierHandle>();
        handA.point = _bPoint;
        _bPoint.handleA = handA;

        if (lastPoint != null)
        {
            GameObject lastB = new GameObject("BézierHandle B", typeof(BezierHandle));
            lastB.transform.position = lastPoint.handleA.transform.position;
            lastB.transform.parent = _bPoint.transform;

            BezierHandle handB = lastB.GetComponent<BezierHandle>();
            handB.point = _bPoint;
            lastPoint.handleB = handB;

            handA.connectedHandle = handB;
            handB.connectedHandle = handA;

            handB.MoveConnectedHandle();

            lastPoint.next = _bPoint;
            _bPoint.prev = lastPoint;
        }
    }

    void LinkToStart()
    {
        BezierPoint startPoint = lastPoint;
        while (startPoint.prev != null)
            startPoint = startPoint.prev;

        lastPoint.next = curPoint;

        GameObject newB = new GameObject("BezierHandle B", typeof(BezierHandle));
        newB.transform.position = lastPoint.handleA.transform.position;
        newB.transform.parent = startPoint.transform;

        BezierHandle handB = newB.GetComponent<BezierHandle>();
        handB.point = startPoint;
        lastPoint.handleB = handB;

        startPoint.handleA.connectedHandle = handB;
        handB.connectedHandle = startPoint.handleA;

        startPoint.bLocked = false;

        lastPoint.next = startPoint;
        startPoint.prev = lastPoint;
    }
}
