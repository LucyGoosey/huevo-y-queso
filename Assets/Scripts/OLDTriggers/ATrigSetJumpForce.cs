using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetJumpForce : MonoBehaviour
{
	public float jumpForce = 500f;

	private float[] jumpForce_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			jumpForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().jumpForce;
			_coll.gameObject.GetComponent<Mario>().jumpForce = jumpForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().jumpForce = jumpForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
