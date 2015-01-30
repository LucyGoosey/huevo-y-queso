using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetPreciseJumpVelocityModifier : MonoBehaviour
{
Vector2 preciseJumpVelocityModifier = Vector2.zero;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().preciseJumpVelocityModifier = preciseJumpVelocityModifier;
	}
}
