using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetDashForce : MonoBehaviour
{
	public Vector2 dashForce = new Vector2(15f, 0f);

	private Vector2[] dashForce_store = new Vector2[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			dashForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().dashForce;
			_coll.gameObject.GetComponent<Mario>().dashForce = dashForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashForce = dashForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
