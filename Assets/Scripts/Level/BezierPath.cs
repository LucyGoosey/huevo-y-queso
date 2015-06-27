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

#if UNITY_EDITOR
    [SerializeField]
    BezierPoint startPoint = null;
#endif

	// Use this for initialization
	void Start ()
    {
        if (startPoint == null)
            CreatePathHandles();
	}
    
	void Update ()
    {
        	
	}

    /// <summary>
    /// Generates the BezierPoint & BezierHandle GUI controllers.
    /// </summary>
    void CreatePathHandles()
    {
        BezierPoint lastPoint;
        lastPoint = null;

        for (int i = 0; i < path.Count; ++i)
        {
            GameObject go = new GameObject(string.Format("Point {0}", i), typeof(BezierPoint));
            go.transform.position = path[i].position;
            go.transform.parent = transform;

            BezierPoint curPoint = go.GetComponent<BezierPoint>();
            if (i == 0)
                startPoint = curPoint;

            if (i > 0)
            {
                curPoint.prev = lastPoint;
                lastPoint.next = curPoint;
            }

            if(!bIsLinked || (i > 1 && bIsLinked))
                AddHandles(curPoint, lastPoint);

            lastPoint = curPoint;
        }

        if (bIsLinked)
            AddHandles(startPoint, lastPoint);
    }

    void AddHandles(BezierPoint _bPoint, BezierPoint lastPoint)
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

    public void GeneratePath()
    {
        for(int i = 0; i < 50; ++i)
            path.Add(new PathPoint(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)),
                                    new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)),
                                    new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100))));
    }
}