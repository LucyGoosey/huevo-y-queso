using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif

public class BezierPath : MonoBehaviour {

    [System.Serializable]
    public class PathPoint
    {
        public Vector3 position = Vector3.zero;

        public Vector3 A = Vector3.zero;
        public Vector3 B = Vector3.zero;

        public PathPoint(Vector3 _location, Vector3 _A, Vector3 _B)
        {
            position = _location;

            A = _A;
            B = _B;
        }
    }

    [SerializeField]
    public List<PathPoint> path = new List<PathPoint>();
    public bool bIsLinked = false;

    #region Editor
    #if UNITY_EDITOR
    [SerializeField]
    BezierAnchor startPoint = null;
    public BezierAnchor StartPoint { get { return startPoint; } set { startPoint = value; } }

	// Use this for initialization
	void Start ()
    {
        if (startPoint != null)
            CreatePathHandles();
        else
        {
            startPoint = new GameObject("Bézier Point 0", typeof(BezierAnchor)).GetComponent<BezierAnchor>();
            startPoint.transform.position = transform.position;
            startPoint.transform.parent = transform;

            startPoint.handleA = new GameObject("Handle A", typeof(BezierHandle)).GetComponent<BezierHandle>();
            startPoint.handleA.transform.position = transform.position;
            startPoint.handleA.transform.parent = startPoint.transform;
            startPoint.handleA.point = startPoint;
        }
	}

    void Update()
    {
        if (path.Count != 0 && startPoint == null)
            CreatePathHandles();
    }

    void OnDestroy()
    {
        ClearPath();
    }

    public void ClearPath()
    {
        BezierAnchor cur = startPoint;
        while (cur != null)
        {
            BezierAnchor next = cur.next;
            DestroyImmediate(cur.gameObject);
            cur = next;
        }

        path = new List<PathPoint>();
    }

    /// <summary>
    /// Generates the BezierPoint & BezierHandle GUI controllers.
    /// </summary>
    public void CreatePathHandles()
    {
        BezierAnchor lastPoint;
        lastPoint = null;

        for (int i = 0; i < path.Count; ++i)
        {
            BezierAnchor curPoint = new GameObject(string.Format("Bézier Point {0}", i), typeof(BezierAnchor))
                                        .GetComponent<BezierAnchor>();

            curPoint.transform.position = path[i].position;
            curPoint.transform.parent = transform;

            if (i == 0)
                startPoint = curPoint;

            if (i > 0)
            {
                curPoint.prev = lastPoint;
                lastPoint.next = curPoint;
            }

            if(!bIsLinked || (i > 1 && bIsLinked))
                curPoint.AddHandles(lastPoint,
                                    path[i].A != Vector3.zero ? path[i].A : (Vector3?)null,
                                    path[i].B != Vector3.zero ? path[i].B : (Vector3?)null);

            lastPoint = curPoint;
        }
    }

    public void RandomPath()
    {
        for(int i = 0; i < 5; ++i)
            path.Add(new PathPoint(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)),
                                    new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)),
                                    new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100))));
    }

    public void SavePositions()
    {
        if (startPoint == null)
            return;

        path = new List<PathPoint>();
        BezierAnchor curPoint = startPoint;

        do{
            Debug.Log(string.Format("Saving point {0}", curPoint));
            path.Add(new PathPoint(curPoint.transform.position,
                                    curPoint.handleA != null ? curPoint.handleA.transform.position : Vector3.zero,
                                    curPoint.handleB != null ? curPoint.handleB.transform.position : Vector3.zero));

            curPoint = curPoint.next;
        } while (curPoint != null && curPoint != startPoint);
    }
    #endif
    #endregion
}