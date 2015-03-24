using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetLongWallKickForce : MonoBehaviour
{
	public float longWallKickForce = 5f;

	private float[] longWallKickForce_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			longWallKickForce_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().longWallKickForce;
			_coll.gameObject.GetComponent<Mario>().longWallKickForce = longWallKickForce;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().longWallKickForce = longWallKickForce_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
