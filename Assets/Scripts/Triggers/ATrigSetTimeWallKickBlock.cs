using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetTimeWallKickBlock : MonoBehaviour
{
	public float timeWallKickBlock = 0.5f;

	private float[] timeWallKickBlock_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			timeWallKickBlock_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().timeWallKickBlock;
			_coll.gameObject.GetComponent<Mario>().timeWallKickBlock = timeWallKickBlock;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().timeWallKickBlock = timeWallKickBlock_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
