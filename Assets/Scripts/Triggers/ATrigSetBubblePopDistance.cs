using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetBubblePopDistance : MonoBehaviour
{
	public float bubblePopDistance = 4f;

	private float[] bubblePopDistance_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			bubblePopDistance_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().bubblePopDistance;
			_coll.gameObject.GetComponent<Mario>().bubblePopDistance = bubblePopDistance;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bubblePopDistance = bubblePopDistance_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
