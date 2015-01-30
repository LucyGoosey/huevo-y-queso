using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGlideKillsY : MonoBehaviour
{
	public bool bGlideKillsY = true;

	private bool[] bGlideKillsY_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bGlideKillsY_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bGlideKillsY;
			_coll.gameObject.GetComponent<Mario>().bGlideKillsY = bGlideKillsY;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bGlideKillsY = bGlideKillsY_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
