using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGroundDragMagic : MonoBehaviour
{
	public float groundDragMagic = 0.05f;

	private float[] groundDragMagic_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			groundDragMagic_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().groundDragMagic;
			_coll.gameObject.GetComponent<Mario>().groundDragMagic = groundDragMagic;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundDragMagic = groundDragMagic_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
