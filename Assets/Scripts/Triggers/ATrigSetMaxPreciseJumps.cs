using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxPreciseJumps : MonoBehaviour
{
	public int maxPreciseJumps = 1;

	private int[] maxPreciseJumps_store = new int[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxPreciseJumps_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxPreciseJumps;
			_coll.gameObject.GetComponent<Mario>().maxPreciseJumps = maxPreciseJumps;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxPreciseJumps = maxPreciseJumps_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
