using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
            Debug.Log("Player in trigger.");
    }
}
