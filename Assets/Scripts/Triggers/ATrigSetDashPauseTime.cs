using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDashPauseTime : MonoBehaviour
{
	public float dashPauseTime = 0.1f;

	private float[] dashPauseTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			dashPauseTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().dashPauseTime;
			_coll.gameObject.GetComponent<Mario>().dashPauseTime = dashPauseTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashPauseTime = dashPauseTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
