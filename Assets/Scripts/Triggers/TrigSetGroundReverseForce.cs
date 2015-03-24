using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetGroundReverseForce : MonoBehaviour
{
float groundReverseForce = 2f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundReverseForce = groundReverseForce;
	}
}
