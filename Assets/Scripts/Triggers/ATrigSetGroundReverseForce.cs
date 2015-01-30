using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGroundReverseForce : MonoBehaviour
{
	public float groundReverseForce = 2f;

	private float[] groundReverseForce_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			groundReverseForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().groundReverseForce;
			_coll.gameObject.GetComponent<Mario>().groundReverseForce = groundReverseForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundReverseForce = groundReverseForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
