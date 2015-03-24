using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetSlamAfterPrecise : MonoBehaviour
{
	public bool bSlamAfterPrecise = true;

	private bool[] bSlamAfterPrecise_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bSlamAfterPrecise_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise;
			_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise = bSlamAfterPrecise;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise = bSlamAfterPrecise_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
