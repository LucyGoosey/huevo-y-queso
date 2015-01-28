using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class MovingPlatform : MonoBehaviour
{
    public List<Transform> path = new List<Transform>();
    public bool bRepeats = false;

    #region Editor
#if UNITY_EDITOR
    private bool bSelected = false;
    private bool bWasSelected = false;
    private List<SpriteRenderer> markers = new List<SpriteRenderer>();    

    private void SetupMarkers()
    {
        if (markers.Count != path.Count)
        {
            if (markers.Count != 0)
                ClearMarkers();

            for (int i = 0; i < path.Count; ++i)
            {
                markers.Add(new GameObject().AddComponent<SpriteRenderer>());
                markers[i].gameObject.name = "__E__Marker";

                markers[i].sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
                markers[i].color = new Color(1, 1, 1, 0.5f);
                markers[i].sortingLayerName = "Editor";

                markers[i].transform.position = path[i].transform.position;
                markers[i].transform.localScale = transform.localScale;
                markers[i].transform.parent = path[i].transform;
            }
        }
    }

    private void ClearMarkers()
    {
        for (int i = 0; i < markers.Count; ++i)
        {
            DestroyImmediate(markers[i].gameObject);
        }

        markers.Clear();
    }

    private void CheckSelection()
    {
        bWasSelected = bSelected;
        // bSelected is set to false, and must face the trials ahead of it to become true again.
        bSelected = false;

        if (Selection.Contains(gameObject))
            bSelected = true;
        else
        {
            for (int i = 0; i < path.Count; ++i)
            {
                if (Selection.Contains(path[i].gameObject))
                {
                    bSelected = true;
                    break;
                }

                if(path[i].childCount != 0)
                    if (Selection.Contains(path[i].GetChild(0).gameObject))
                    {
                        Selection.activeInstanceID = path[i].gameObject.GetInstanceID();
                        bSelected = true;
                        break;
                    }
            }
        }

        if (!bWasSelected && bSelected)
            SetupMarkers();
        else if (bWasSelected && !bSelected)
            ClearMarkers();
    }

    void OnDrawGizmos()
    {
        CheckSelection();

        if (bSelected)
            DrawGizmosSelected();
    }

    void DrawGizmosSelected()
    {
        if (path.Count == 0)
            return;

        GizmoDrawArrowedLine(transform.position, path[0].position);
        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; ++i)
                GizmoDrawArrowedLine(path[i - 1].position, path[i].position);
            if (bRepeats)
                GizmoDrawArrowedLine(path[path.Count - 1].position, transform.position);
        }
    }

    void GizmoDrawArrowedLine(Vector3 start, Vector3 end, float length = 1f, float angle = 30f)
    {
        Gizmos.color = new Color(0.66f, 0.66f, 0.66f);
        Gizmos.DrawLine(start, end);

        Gizmos.color = new Color(0.75f, 0.75f, 0.75f);
        Vector3 midPoint = Vector3.Lerp(start, end, 0.5f);
        Vector3 lineDir = (start - end).normalized; // Get the inverted direction of the line
        Gizmos.DrawLine(midPoint, midPoint + (Quaternion.Euler(new Vector3(0f, 0f, angle)) * lineDir * length));
        Gizmos.DrawLine(midPoint, midPoint + (Quaternion.Euler(new Vector3(0f, 0f, -angle)) * lineDir * length));
    }
#endif
    #endregion
}
