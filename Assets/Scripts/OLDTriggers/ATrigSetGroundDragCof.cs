using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGroundDragCof : MonoBehaviour
{
	public float groundDragCof = 1f;

	private float[] groundDragCof_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			groundDragCof_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().groundDragCof;
			_coll.gameObject.GetComponent<Mario>().groundDragCof = groundDragCof;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundDragCof = groundDragCof_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
