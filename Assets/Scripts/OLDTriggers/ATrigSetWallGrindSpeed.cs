using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetWallGrindSpeed : MonoBehaviour
{
	public float wallGrindSpeed = 5f;

	private float[] wallGrindSpeed_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			wallGrindSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().wallGrindSpeed;
			_coll.gameObject.GetComponent<Mario>().wallGrindSpeed = wallGrindSpeed;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().wallGrindSpeed = wallGrindSpeed_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
