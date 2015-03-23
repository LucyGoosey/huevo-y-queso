using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class TrigAddForce : MonoBehaviour
{
	public Vector2 addForce = Vector2.zero;
    public Vector2 velocityModifier = new Vector2(1f, 1f);

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
		{
			_coll.gameObject.GetComponent<Mario>().rigidbody2D.velocity = Vector2.Scale(_coll.gameObject.GetComponent<Mario>().rigidbody2D.velocity, velocityModifier);
            _coll.gameObject.GetComponent<Mario>().rigidbody2D.AddForce(addForce);
		}
	}
}
