using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetExtraJumpStopsFall : MonoBehaviour
{
	public bool bExtraJumpStopsFall = true;

	private bool[] bExtraJumpStopsFall_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bExtraJumpStopsFall_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall;
			_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall = bExtraJumpStopsFall;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall = bExtraJumpStopsFall_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
