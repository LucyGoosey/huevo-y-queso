using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetDeadMoveSpeed : MonoBehaviour
{
float deadMoveSpeed = 15f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().deadMoveSpeed = deadMoveSpeed;
	}
}
