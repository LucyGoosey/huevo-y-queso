using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchSlamAfterPrecise : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise = !_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise = !_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise;
	}
}
