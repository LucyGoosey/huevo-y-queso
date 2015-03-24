using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetDeadAccelSpeed : MonoBehaviour
{
float deadAccelSpeed = 40f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().deadAccelSpeed = deadAccelSpeed;
	}
}
