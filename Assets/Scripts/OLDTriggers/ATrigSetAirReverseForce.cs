using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetAirReverseForce : MonoBehaviour
{
	public float airReverseForce = 2.5f;

	private float[] airReverseForce_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			airReverseForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().airReverseForce;
			_coll.gameObject.GetComponent<Mario>().airReverseForce = airReverseForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airReverseForce = airReverseForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
