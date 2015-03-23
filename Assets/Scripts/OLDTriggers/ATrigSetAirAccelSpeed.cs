using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetAirAccelSpeed : MonoBehaviour
{
	public float airAccelSpeed = 10f;

	private float[] airAccelSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			airAccelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().airAccelSpeed;
			_coll.gameObject.GetComponent<Mario>().airAccelSpeed = airAccelSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airAccelSpeed = airAccelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
