using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGlideGravityScale : MonoBehaviour
{
	public float glideGravityScale = 0.5f;

	private float[] glideGravityScale_store = new float[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			glideGravityScale_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().glideGravityScale;
			_coll.gameObject.GetComponent<Mario>().glideGravityScale = glideGravityScale;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().glideGravityScale = glideGravityScale_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
