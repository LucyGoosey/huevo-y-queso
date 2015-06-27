using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif

public class BezierPathWindow : EditorWindow 
{
    public bool bIsPlacing = false;

    private BezierPath path = null;

    private uint numPoints = 0;

    private BezierAnchor lastPoint = null;
    private BezierAnchor curPoint = null;

    public void Setup(BezierPath _path, uint _numPoints, BezierAnchor _lastPoint)
    {
        path = _path;
        numPoints = _numPoints;
        lastPoint = _lastPoint;
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
            HandlePlacement(sceneView);
    }

    #region Placment
    private void HandlePlacement(SceneView _sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event cur = Event.current;

        if (cur.button != 0)
            return;

        switch (cur.type)
        {
            case EventType.MouseDown:
                PlaceAnchor(_sceneView, cur.mousePosition);
                cur.Use();
                break;

            case EventType.MouseDrag:
                curPoint.handleA.transform.position = GetClickPos(cur.mousePosition, _sceneView);
                break;

            case EventType.MouseUp:
                lastPoint = curPoint;
                curPoint = null;
                break;

            default:
                break;
        }
    }

    private void PlaceAnchor(SceneView _sceneView, Vector2 _mousePosition)
    {
        curPoint = new GameObject(string.Format("Bézier Point {0}", numPoints), typeof(BezierAnchor))
                        .GetComponent<BezierAnchor>();

        if (numPoints == 0)
            path.StartPoint = curPoint;

        ++numPoints;

        curPoint.transform.position = GetClickPos(_mousePosition, _sceneView);
        curPoint.transform.parent = path.transform;

        curPoint.bLocked = curPoint.bDistanceLocked = true;
        curPoint.AddHandles(lastPoint);
    }
    #endregion

    Vector3 GetClickPos(Vector2 mousePos, SceneView sceneView)
    {
        mousePos.y = sceneView.camera.pixelHeight - mousePos.y;

        Vector3 worldPos = sceneView.camera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        return worldPos;
    }

    void LinkToStart()
    {
        BezierAnchor startPoint = lastPoint;
        while (startPoint.prev != null)
            startPoint = startPoint.prev;
        
        lastPoint.next = curPoint;

        GameObject newB = new GameObject("Bézier Handle B", typeof(BezierHandle));
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
