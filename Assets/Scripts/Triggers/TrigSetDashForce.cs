using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetDashForce : MonoBehaviour
{
Vector2 dashForce = new Vector2(15f, 0f);

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashForce = dashForce;
	}
}
