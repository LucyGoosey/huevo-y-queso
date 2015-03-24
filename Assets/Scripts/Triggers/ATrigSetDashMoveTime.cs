using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDashMoveTime : MonoBehaviour
{
	public float dashMoveTime = 0.3f;

	private float[] dashMoveTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			dashMoveTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().dashMoveTime;
			_coll.gameObject.GetComponent<Mario>().dashMoveTime = dashMoveTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashMoveTime = dashMoveTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
