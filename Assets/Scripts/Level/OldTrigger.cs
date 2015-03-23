using UnityEngine;
using System.Collections;

public class OldTrigger : MonoBehaviour
{

    public MovinPlatform[] platforms;

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (platforms == null)
            return;

        if (_coll.tag == "Player")
            for (int i = 0; i < platforms.Length; ++i)
                platforms[i].bShouldMove = true;
    }
}