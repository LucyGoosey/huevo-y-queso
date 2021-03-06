using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchDashBlocksInput : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bDashBlocksInput = !_coll.gameObject.GetComponent<Mario>().bDashBlocksInput;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bDashBlocksInput = !_coll.gameObject.GetComponent<Mario>().bDashBlocksInput;
	}
}
