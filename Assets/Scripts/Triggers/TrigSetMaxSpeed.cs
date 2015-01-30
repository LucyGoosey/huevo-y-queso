using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxSpeed : MonoBehaviour
{
Vector2 maxSpeed = new Vector2(7.5f, 10f);

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSpeed = maxSpeed;
	}
}
