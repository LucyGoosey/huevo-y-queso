using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetShouldDashPause : MonoBehaviour
{
	public bool bShouldDashPause = false;

	private bool[] bShouldDashPause_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bShouldDashPause_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bShouldDashPause;
			_coll.gameObject.GetComponent<Mario>().bShouldDashPause = bShouldDashPause;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bShouldDashPause = bShouldDashPause_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
