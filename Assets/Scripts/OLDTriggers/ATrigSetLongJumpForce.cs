using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetLongJumpForce : MonoBehaviour
{
	public float longJumpForce = 0.1f;

	private float[] longJumpForce_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			longJumpForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().longJumpForce;
			_coll.gameObject.GetComponent<Mario>().longJumpForce = longJumpForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().longJumpForce = longJumpForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
