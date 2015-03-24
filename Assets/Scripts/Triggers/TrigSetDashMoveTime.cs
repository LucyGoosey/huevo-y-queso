using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetDashMoveTime : MonoBehaviour
{
float dashMoveTime = 0.3f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().dashMoveTime = dashMoveTime;
	}
}
