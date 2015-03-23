using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchExtraJumpStopsFall : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall = !_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall;
	}
}
