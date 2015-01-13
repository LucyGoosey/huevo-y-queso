using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Vector2 dir;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += (Vector3.right * 5f) * Time.deltaTime;
	}
}
