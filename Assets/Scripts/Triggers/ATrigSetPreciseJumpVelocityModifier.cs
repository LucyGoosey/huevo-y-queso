using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSetPreciseJumpVelocityModifier : MonoBehaviour
{
	public Vector2 preciseJumpVelocityModifier = Vector2.zero;

	private Vector2[] preciseJumpVelocityModifier_store = new Vector2[4];

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			preciseJumpVelocityModifier_store[_coll.gameObject.GetComponent<Mario>().playerNum] = _coll.gameObject.GetComponent<Mario>().preciseJumpVelocityModifier;
			_coll.gameObject.GetComponent<Mario>().preciseJumpVelocityModifier = preciseJumpVelocityModifier;
		}
	}

	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().preciseJumpVelocityModifier = preciseJumpVelocityModifier_store[_coll.gameObject.GetComponent<Mario>().playerNum];
	}
}
