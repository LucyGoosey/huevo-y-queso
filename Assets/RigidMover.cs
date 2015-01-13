using UnityEngine;
using System.Collections;

public class RigidMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidbody2D.velocity = (Vector3.right * 5f);
	}
}
