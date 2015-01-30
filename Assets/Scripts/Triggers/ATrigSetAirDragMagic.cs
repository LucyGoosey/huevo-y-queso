using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetAirDragMagic : MonoBehaviour
{
	public float airDragMagic = 0f;

	private float[] airDragMagic_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			airDragMagic_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().airDragMagic;
			_coll.gameObject.GetComponent<Mario>().airDragMagic = airDragMagic;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airDragMagic = airDragMagic_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
