using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchFloatWhileSliding : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = !_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = !_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding;
	}
}
