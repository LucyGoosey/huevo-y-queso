using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxFramesToForgiveJump : MonoBehaviour
{
	public int maxFramesToForgiveJump = 3;

	private int[] maxFramesToForgiveJump_store = new int[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxFramesToForgiveJump_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxFramesToForgiveJump;
			_coll.gameObject.GetComponent<Mario>().maxFramesToForgiveJump = maxFramesToForgiveJump;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxFramesToForgiveJump = maxFramesToForgiveJump_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
