using UnityEngine;

public class Trigger : BaseTrigger
{
    void OnTriggerEnter2D(Collider2D _coll)
    {
        if(_coll.tag == "Player")
            OnTrigger(_coll.GetComponent<Huevo>());
    }
}
