using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Attachable : MonoBehaviour
{
    public List<Huevo> attached = new List<Huevo>();

    void OnTriggerEnter2D(Collider2D _coll)
    {
        Debug.Log("Hit by: " + _coll.name);
        if (_coll.tag == "Hand")
            Attach(_coll.transform.parent.GetComponent<Huevo>());
    }

    virtual protected void Attach(Huevo _h)
    {
        if (!attached.Contains(_h))
        {
            attached.Add(_h);
            _h.AttachToObject(this);
        }
    }

    virtual protected void Detach(Huevo _h)
    {
        if(attached.Contains(_h))
        {
            attached.Remove(_h);
            _h.DetachFromObject();
        }
    }
}
