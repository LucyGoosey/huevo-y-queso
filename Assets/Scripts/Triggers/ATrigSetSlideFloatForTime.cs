using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetSlideFloatForTime : MonoBehaviour
{
	public bool bSlideFloatForTime = true;

	private bool[] bSlideFloatForTime_store = new bool[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bSlideFloatForTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bSlideFloatForTime;
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = bSlideFloatForTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = bSlideFloatForTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
