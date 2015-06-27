using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif

public class BezierHandle : MonoBehaviour {

    public BezierPoint point;

    public BezierHandle connectedHandle;

#if UNITY_EDITOR
    private Vector3 lastKnownPos;

    void Start()
    {
        lastKnownPos = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (point == null)
            DestroyImmediate(gameObject);

        if (transform.position != lastKnownPos)
            MoveConnectedHandle();
	}

    void OnDrawGizmos()
    {
        if (point == null)
            return;

        Gizmos.color = new Color(0.625f, 0.75f, 0.15f, 0.33f);
        Gizmos.DrawLine(transform.position, point.transform.position);
    }

    public void MoveConnectedHandle()
    {
        if (point != null && point.bLocked && connectedHandle != null)
        {
            Vector3 move = transform.position - point.transform.position;

            if (!point.bDistanceLocked)
                move = move.normalized * Vector3.Distance(point.transform.position, connectedHandle.transform.position);

            connectedHandle.transform.position = point.transform.position - move;
            connectedHandle.UpdateLastKnownPos();
        }

        UpdateLastKnownPos();
    }

    public void UpdateLastKnownPos()
    {
        lastKnownPos = transform.position;
    }
#endif
}