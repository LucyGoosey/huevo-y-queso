using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchFloatWhileSliding : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = !_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding;
	}
}
