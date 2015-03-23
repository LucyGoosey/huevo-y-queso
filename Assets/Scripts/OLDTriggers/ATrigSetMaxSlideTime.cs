using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxSlideTime : MonoBehaviour
{
	public float maxSlideTime = 1f;

	private float[] maxSlideTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxSlideTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxSlideTime;
			_coll.gameObject.GetComponent<Mario>().maxSlideTime = maxSlideTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSlideTime = maxSlideTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
