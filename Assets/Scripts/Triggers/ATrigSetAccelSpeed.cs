using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetAccelSpeed : MonoBehaviour
{
	public float accelSpeed = 20f;

	private float[] accelSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			accelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().accelSpeed;
			_coll.gameObject.GetComponent<Mario>().accelSpeed = accelSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().accelSpeed = accelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
