using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchExtraJumpStopsFall : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall = !_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall = !_coll.gameObject.GetComponent<Mario>().bExtraJumpStopsFall;
	}
}
