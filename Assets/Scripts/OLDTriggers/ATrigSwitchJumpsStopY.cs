using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchJumpsStopY : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bJumpsStopY = !_coll.gameObject.GetComponent<Mario>().bJumpsStopY;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bJumpsStopY = !_coll.gameObject.GetComponent<Mario>().bJumpsStopY;
	}
}
