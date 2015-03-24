using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxFramesToForgiveJump : MonoBehaviour
{
int maxFramesToForgiveJump = 3;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxFramesToForgiveJump = maxFramesToForgiveJump;
	}
}
