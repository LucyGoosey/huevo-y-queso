using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDeadAccelSpeed : MonoBehaviour
{
	public float deadAccelSpeed = 40f;

	private float[] deadAccelSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			deadAccelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().deadAccelSpeed;
			_coll.gameObject.GetComponent<Mario>().deadAccelSpeed = deadAccelSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().deadAccelSpeed = deadAccelSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
