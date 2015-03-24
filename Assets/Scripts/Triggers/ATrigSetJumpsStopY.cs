using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetJumpsStopY : MonoBehaviour
{
	public bool bJumpsStopY = true;

	private bool[] bJumpsStopY_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bJumpsStopY_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bJumpsStopY;
			_coll.gameObject.GetComponent<Mario>().bJumpsStopY = bJumpsStopY;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bJumpsStopY = bJumpsStopY_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
