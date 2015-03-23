using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetJumpForce : MonoBehaviour
{
float jumpForce = 500f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().jumpForce = jumpForce;
	}
}
