using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxExtraJumps : MonoBehaviour
{
	public int maxExtraJumps = 1;

	private int[] maxExtraJumps_store = new int[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxExtraJumps_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxExtraJumps;
			_coll.gameObject.GetComponent<Mario>().maxExtraJumps = maxExtraJumps;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxExtraJumps = maxExtraJumps_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
