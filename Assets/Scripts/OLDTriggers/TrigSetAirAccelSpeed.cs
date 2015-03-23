using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetAirAccelSpeed : MonoBehaviour
{
float airAccelSpeed = 10f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airAccelSpeed = airAccelSpeed;
	}
}
