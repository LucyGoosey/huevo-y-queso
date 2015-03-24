using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDeadMoveSpeed : MonoBehaviour
{
	public float deadMoveSpeed = 15f;

	private float[] deadMoveSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			deadMoveSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().deadMoveSpeed;
			_coll.gameObject.GetComponent<Mario>().deadMoveSpeed = deadMoveSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().deadMoveSpeed = deadMoveSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
