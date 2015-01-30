using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMinSlideFloatSpeed : MonoBehaviour
{
	public float minSlideFloatSpeed = 0.5f;

	private float[] minSlideFloatSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			minSlideFloatSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().minSlideFloatSpeed;
			_coll.gameObject.GetComponent<Mario>().minSlideFloatSpeed = minSlideFloatSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().minSlideFloatSpeed = minSlideFloatSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
