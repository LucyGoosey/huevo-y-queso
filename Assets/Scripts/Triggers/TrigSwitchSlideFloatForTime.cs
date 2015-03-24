using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchSlideFloatForTime : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = !_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime;
	}
}
