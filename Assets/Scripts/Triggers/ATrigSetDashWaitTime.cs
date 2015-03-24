using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDashWaitTime : MonoBehaviour
{
	public float dashWaitTime = 1f;

	private float[] dashWaitTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			dashWaitTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().dashWaitTime;
			_coll.gameObject.GetComponent<Mario>().dashWaitTime = dashWaitTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashWaitTime = dashWaitTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
