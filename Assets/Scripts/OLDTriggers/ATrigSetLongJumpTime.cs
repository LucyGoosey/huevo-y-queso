using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetLongJumpTime : MonoBehaviour
{
	public float longJumpTime = 0.5f;

	private float[] longJumpTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			longJumpTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().longJumpTime;
			_coll.gameObject.GetComponent<Mario>().longJumpTime = longJumpTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().longJumpTime = longJumpTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
