using UnityEngine;
using System.Collections;

public class MovinPlatform : MonoBehaviour {

    public Transform target;
    public float t = 0.0f;
    public float speed = 0.01f;
    public bool bShouldMove = false;
    public Vector3 startpos;
    public bool bShouldRepeat;

	// Use this for initialization
	void Start () {
        startpos = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
        if (bShouldMove)
        {
            transform.position = Vector2.Lerp(startpos, target.position, t);
            t += speed;
            if (t >= 1 || t <= 0)
            {if (bShouldRepeat)
                speed = -speed;
            }
        }
	}

    void OnTriggerEnter()
    {

    }
}
