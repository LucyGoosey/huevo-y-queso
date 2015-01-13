using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Vine : MonoBehaviour {
#if UNITY_EDITOR
    public bool bDrawVineSpheres = true;
    public float sphereRadius = 0.05f;
#endif

    public int numPoints = 0;
    public float distanceBetweenPoints = 0.1f;

    VinePiece[] points = new VinePiece[0];
    LineRenderer lRenderer;

    void Start()
    {

        lRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (numPoints != points.Length)
            UpdatePoints();

        for (int i = 0; i < points.Length; ++i)
            lRenderer.SetPosition(i, points[i].transform.position);
    }

    void UpdatePoints()
    {
        Debug.Log("Updating points");
        ClearPoints();

        points = new VinePiece[numPoints];
        for (int i = 0; i < numPoints; ++i)
        {
            points[i] = new GameObject().AddComponent<VinePiece>();
            points[i].transform.position = transform.position;
            points[i].transform.parent = transform;

            points[i].distance = distanceBetweenPoints;

            if (i != 0)
            {
                points[i].transform.position = points[i - 1].transform.position - (Vector3.up * distanceBetweenPoints);
                points[i].connectedTo = points[i - 1];
                points[i - 1].nextVine = points[i];
            }
            else
                points[i].rigidbody2D.isKinematic = true;
        }
        
        lRenderer.SetVertexCount(points.Length);
    }

    private void ClearPoints()
    {
        for (int i = 0; i < points.Length; ++i)
            if (points[i] != null)
                DestroyImmediate(points[i].gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!bDrawVineSpheres)
            return;

        Gizmos.color = Color.green;
        Vector2 position = transform.position;
        for (int i = 0; i < numPoints; ++i)
        {
            Gizmos.DrawSphere(position, sphereRadius);
            position.y -= distanceBetweenPoints;
        }
    }
#endif
}
