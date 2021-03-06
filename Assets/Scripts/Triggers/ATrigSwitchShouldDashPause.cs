using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchShouldDashPause : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bShouldDashPause = !_coll.gameObject.GetComponent<Mario>().bShouldDashPause;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bShouldDashPause = !_coll.gameObject.GetComponent<Mario>().bShouldDashPause;
	}
}
