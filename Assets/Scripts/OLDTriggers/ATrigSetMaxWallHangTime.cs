using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetMaxWallHangTime : MonoBehaviour
{
	public float maxWallHangTime = 2.5f;

	private float[] maxWallHangTime_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			maxWallHangTime_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().maxWallHangTime;
			_coll.gameObject.GetComponent<Mario>().maxWallHangTime = maxWallHangTime;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxWallHangTime = maxWallHangTime_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
