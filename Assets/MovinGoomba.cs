using UnityEngine;
using System.Collections;

public class MovinGoomba : Enemy {

    public Transform target;
    public float t = 0.0f;
    public float speed = 0.01f;
    public bool bShouldMove = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (bShouldMove)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, t);
            t += speed;
            if (t >= 1||t<=0)
                speed=-speed;
        }
	}

    void OnTriggerEnter()
    {

    }
}
