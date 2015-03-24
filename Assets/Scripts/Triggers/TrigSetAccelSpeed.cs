using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetAccelSpeed : MonoBehaviour
{
float accelSpeed = 20f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().accelSpeed = accelSpeed;
	}
}
