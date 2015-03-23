using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetFloatWhileSliding : MonoBehaviour
{
	public bool bFloatWhileSliding = false;

	private bool[] bFloatWhileSliding_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bFloatWhileSliding_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bFloatWhileSliding;
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = bFloatWhileSliding;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = bFloatWhileSliding_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
