using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Attachable : MonoBehaviour
{
    public class AttachableHuevo
    {
        public Huevo huevo;

        public AttachableHuevo(Huevo _h)
        {
            huevo = _h;
        }
    }

    public List<AttachableHuevo> attached = new List<AttachableHuevo>();

    void OnTriggerEnter2D(Collider2D _coll)
    {
        Debug.Log("Hit by: " + _coll.name);
        if (_coll.tag == "Hand")
            Attach(_coll.transform.parent.GetComponent<Huevo>());
    }

    virtual protected void Attach(Huevo _h)
    {
        if (IsHuevoAttached(_h) == null)
        {
            attached.Add(new AttachableHuevo(_h));
            _h.AttachToObject(this);
        }
    }

    virtual protected void Detach(Huevo _h)
    {
        AttachableHuevo ah = IsHuevoAttached(_h);
        if(ah != null)
        {
            attached.Remove(ah);
            ah.huevo.DetachFromObject();
        }
    }

    protected AttachableHuevo IsHuevoAttached(Huevo _h)
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i].huevo == _h)
                return attached[i];
        return null;
    }
}
