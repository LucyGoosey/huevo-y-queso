using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxSlideFloatTime : MonoBehaviour
{
	public float maxSlideFloatTime = 0.5f;

	private float[] maxSlideFloatTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxSlideFloatTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxSlideFloatTime;
			_coll.gameObject.GetComponent<Mario>().maxSlideFloatTime = maxSlideFloatTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSlideFloatTime = maxSlideFloatTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
