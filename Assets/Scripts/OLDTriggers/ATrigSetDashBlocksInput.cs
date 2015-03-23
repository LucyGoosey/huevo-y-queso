using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDashBlocksInput : MonoBehaviour
{
	public bool bDashBlocksInput = false;

	private bool[] bDashBlocksInput_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bDashBlocksInput_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bDashBlocksInput;
			_coll.gameObject.GetComponent<Mario>().bDashBlocksInput = bDashBlocksInput;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bDashBlocksInput = bDashBlocksInput_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
