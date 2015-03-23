using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxSpeed : MonoBehaviour
{
	public Vector2 maxSpeed = new Vector2(7.5f, 10f);

	private Vector2[] maxSpeed_store = new Vector2[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxSpeed;
			_coll.gameObject.GetComponent<Mario>().maxSpeed = maxSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSpeed = maxSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
