using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxGlideTime : MonoBehaviour
{
	public float maxGlideTime = 2f;

	private float[] maxGlideTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxGlideTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxGlideTime;
			_coll.gameObject.GetComponent<Mario>().maxGlideTime = maxGlideTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxGlideTime = maxGlideTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
