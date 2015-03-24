using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxDashes : MonoBehaviour
{
	public int maxDashes = 2;

	private int[] maxDashes_store = new int[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxDashes_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxDashes;
			_coll.gameObject.GetComponent<Mario>().maxDashes = maxDashes;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxDashes = maxDashes_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
