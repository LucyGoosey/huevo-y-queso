using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetWallKickForce : MonoBehaviour
{
Vector2 wallKickForce = new Vector2(300f, 250f);

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().wallKickForce = wallKickForce;
	}
}
