#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;

[ExecuteInEditMode]
public class BezierAnchor : MonoBehaviour
{
    public BezierAnchor prev;
    public BezierAnchor next;

    public bool bLocked = false;
    public bool bDistanceLocked = false;

    public BezierHandle handleA;
    public BezierHandle handleB;

	void Update ()
    {
        if (!bLocked)
            bDistanceLocked = false;
    }

    void OnDrawGizmos()
    {
        if (next == null
            || handleA == null || handleB == null)
            return;

        Handles.DrawBezier(transform.position, next.transform.position,
                            handleA.transform.position, handleB.transform.position,
                            Color.green, null, 1f);
    }

    public void AddHandles(BezierAnchor lastPoint)
    {
        handleA = new GameObject("Handle A", typeof(BezierHandle)).GetComponent<BezierHandle>();
        handleA.transform.position = transform.position;
        handleA.transform.parent = transform;

        handleA.point = this;

        if (lastPoint == null )
            return;

        BezierHandle lastB = new GameObject("Handle B", typeof(BezierHandle)).GetComponent<BezierHandle>();
        lastB.transform.position = transform.position;
        lastB.transform.parent = transform;

        lastB.point = this;
        lastPoint.handleB = lastB;

        handleA.connectedHandle = lastB;
        lastB.connectedHandle = handleA;

        lastPoint.next = this;
        prev = lastPoint;
    }

    public void AddHandles(BezierAnchor lastPoint, Vector3? aPos, Vector3? bPos)
    {
        if (aPos != null)
        {
            GameObject newA = new GameObject("Handle A", typeof(BezierHandle));
            newA.transform.parent = transform;
            newA.transform.position = (Vector3)aPos;

            BezierHandle A = newA.GetComponent<BezierHandle>();
            A.point = this;
            handleA = A;
        }

        if (bPos != null)
        {
            GameObject newB = new GameObject("Handle B", typeof(BezierHandle));
            newB.transform.position = (Vector3)bPos;
            handleB = newB.GetComponent<BezierHandle>();
        }

        if (lastPoint == null || lastPoint.handleB == null)
            return;

        lastPoint.handleB.transform.parent = transform;
        lastPoint.handleB.point = this;
    }
}
#endif