using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetEndSlideFloatSpeed : MonoBehaviour
{
	public float endSlideFloatSpeed = 0.2f;

	private float[] endSlideFloatSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			endSlideFloatSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().endSlideFloatSpeed;
			_coll.gameObject.GetComponent<Mario>().endSlideFloatSpeed = endSlideFloatSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().endSlideFloatSpeed = endSlideFloatSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
