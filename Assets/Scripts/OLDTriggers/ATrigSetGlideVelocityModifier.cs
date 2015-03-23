using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetGlideVelocityModifier : MonoBehaviour
{
	public Vector2 glideVelocityModifier = new Vector2(0.5f, 0.5f);

	private Vector2[] glideVelocityModifier_store = new Vector2[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			glideVelocityModifier_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().glideVelocityModifier;
			_coll.gameObject.GetComponent<Mario>().glideVelocityModifier = glideVelocityModifier;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().glideVelocityModifier = glideVelocityModifier_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
