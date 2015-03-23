using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetWallKickForce : MonoBehaviour
{
	public Vector2 wallKickForce = new Vector2(300f, 250f);

	private Vector2[] wallKickForce_store = new Vector2[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			wallKickForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().wallKickForce;
			_coll.gameObject.GetComponent<Mario>().wallKickForce = wallKickForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().wallKickForce = wallKickForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
