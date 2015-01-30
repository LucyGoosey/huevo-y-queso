using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetSlamDoublePressTime : MonoBehaviour
{
	public float slamDoublePressTime = 0.1f;

	private float[] slamDoublePressTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			slamDoublePressTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().slamDoublePressTime;
			_coll.gameObject.GetComponent<Mario>().slamDoublePressTime = slamDoublePressTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().slamDoublePressTime = slamDoublePressTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
