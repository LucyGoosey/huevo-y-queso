using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Attachable : MonoBehaviour
{
    public class AttachableHuevo
    {
        public Huevo huevo;

        public float leftFor = 0f;
        public bool bHoldingJump = false;

        public float[] floatParams = null;

        public AttachableHuevo(Huevo _h) { huevo = _h; }

        public void SetNumFloatParams(uint _numParams) { floatParams = new float[_numParams]; }
    }

    public List<AttachableHuevo> attached = new List<AttachableHuevo>();
    private List<AttachableHuevo> detached = new List<AttachableHuevo>();

    public float timeBeforeDetach = 0.5f;

    protected void Update()
    {
        for (int i = 0; i < detached.Count; ++i)
        {
            detached[i].leftFor -= Time.deltaTime;

            if (detached[i].leftFor <= 0f)
                detached.RemoveAt(i--);
        }
    }

    protected void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i].bHoldingJump && !attached[i].huevo.InHandler.Jump.bHeld)
                attached[i].bHoldingJump = false;
    }

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "Hand")
            Attach(_coll.transform.parent.GetComponent<Huevo>());
    }

    void OnTriggerStay2D(Collider2D _coll)
    {
        if(_coll.tag == "Hand")
            Attach(_coll.transform.parent.GetComponent<Huevo>());
    }

    virtual protected AttachableHuevo Attach(Huevo _h)
    {
        AttachableHuevo ah = null;

        if (IsHuevoAttached(_h) == null)
        {
            ah = new AttachableHuevo(_h);

            if (ah.huevo.InHandler.Jump.bHeld)
                ah.bHoldingJump = true;

            attached.Add(ah);
            ah.huevo.AttachToObject(this);
        }

        return ah;
    }

    virtual protected void Detach(Huevo _h)
    {
        AttachableHuevo ah = IsHuevoAttached(_h);
        if(ah != null && !ah.bHoldingJump)
        {
            ah.leftFor = timeBeforeDetach;
            detached.Add(ah);
            attached.Remove(ah);
            ah.huevo.DetachFromObject();
        }
    }

    protected AttachableHuevo IsHuevoAttached(Huevo _h)
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i].huevo == _h)
                return attached[i];
        for (int i = 0; i < detached.Count; ++i)
            if (detached[i].huevo == _h)
                return detached[i];
        return null;
    }
}
