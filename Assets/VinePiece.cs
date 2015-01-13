using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class VinePiece : MonoBehaviour {
    public DistanceJoint2D joint;
    public float distance = 0.1f;
    public VinePiece connectedTo;
    public VinePiece nextVine = null;

    private DistanceJoint2D reverseJoint = null;

    public VinePiece GetAnchor()
    {
        if (connectedTo == null)
            return this;

        VinePiece anchor = connectedTo;
        while (anchor.connectedTo != null)
            anchor = anchor.connectedTo;

        return anchor;
    }

    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.distance = distance;

        if(connectedTo != null)
            joint.connectedBody = connectedTo.rigidbody2D;

        reverseJoint = gameObject.AddComponent<DistanceJoint2D>();
        reverseJoint.distance = distance;
        reverseJoint.enabled = false;

        gameObject.layer = LayerMask.NameToLayer("Vine");

        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        coll.radius = distance / 2f;
        coll.isTrigger = true;

        rigidbody2D.drag = 0.5f;
    }

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag != "Player")
            return;

        Mario m = _coll.gameObject.transform.parent.gameObject.GetComponent<Mario>();
        //m.VineHit(this);
    }

    public void EnableReverseJoint()
    {
        if (nextVine == null)
            return;

        reverseJoint.connectedBody = nextVine.rigidbody2D;
        reverseJoint.distance = distance;
        reverseJoint.enabled = true;
    }
}
