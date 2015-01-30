using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetGlideVelocityModifier : MonoBehaviour
{
Vector2 glideVelocityModifier = new Vector2(0.5f, 0.5f);

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().glideVelocityModifier = glideVelocityModifier;
	}
}
