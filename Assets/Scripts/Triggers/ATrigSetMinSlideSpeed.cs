using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMinSlideSpeed : MonoBehaviour
{
	public float minSlideSpeed = 0.75f;

	private float[] minSlideSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			minSlideSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().minSlideSpeed;
			_coll.gameObject.GetComponent<Mario>().minSlideSpeed = minSlideSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().minSlideSpeed = minSlideSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
