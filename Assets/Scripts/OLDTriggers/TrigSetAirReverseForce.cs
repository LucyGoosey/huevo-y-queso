using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetAirReverseForce : MonoBehaviour
{
float airReverseForce = 2.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airReverseForce = airReverseForce;
	}
}
