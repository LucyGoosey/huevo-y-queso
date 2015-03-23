using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ATrigSwitchSlideFloatForTime : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = !_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime;
	}
	void OnTriggerExit2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = !_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime;
	}
}
