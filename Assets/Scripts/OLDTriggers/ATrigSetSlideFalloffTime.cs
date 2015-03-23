using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetSlideFalloffTime : MonoBehaviour
{
	public float slideFalloffTime = 0.3f;

	private float[] slideFalloffTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			slideFalloffTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().slideFalloffTime;
			_coll.gameObject.GetComponent<Mario>().slideFalloffTime = slideFalloffTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().slideFalloffTime = slideFalloffTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
