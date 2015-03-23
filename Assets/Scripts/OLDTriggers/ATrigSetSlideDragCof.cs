using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetSlideDragCof : MonoBehaviour
{
	public float slideDragCof = 0.75f;

	private float[] slideDragCof_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			slideDragCof_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().slideDragCof;
			_coll.gameObject.GetComponent<Mario>().slideDragCof = slideDragCof;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().slideDragCof = slideDragCof_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
