using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif

public class BezierPoint : MonoBehaviour
{
    public BezierPoint prev;
    public BezierPoint next;

    public bool bLocked = true;
    public bool bDistanceLocked = true;

    public BezierHandle handleA;
    public BezierHandle handleB;

    #region Editor
#if UNITY_EDITOR
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

        Handles.DrawBezier(transform.position, next.transform.position, handleA.transform.position, handleB.transform.position, Color.green, null, 1f);
    }
#endif

    #endregion Editor
}
