using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetStandStillSpeed : MonoBehaviour
{
float standStillSpeed = 0.01f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().standStillSpeed = standStillSpeed;
	}
}
