using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchJumpsStopY : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bJumpsStopY = !_coll.gameObject.GetComponent<Mario>().bJumpsStopY;
	}
}
