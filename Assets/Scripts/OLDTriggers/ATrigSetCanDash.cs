using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetCanDash : MonoBehaviour
{
	public bool bCanDash = true;

	private bool[] bCanDash_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bCanDash_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bCanDash;
			_coll.gameObject.GetComponent<Mario>().bCanDash = bCanDash;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bCanDash = bCanDash_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
