using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TrigDeathZone : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
            _coll.gameObject.GetComponent<Huevo>().OnKill();
    }
}
