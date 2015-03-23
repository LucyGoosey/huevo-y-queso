using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchSlamAfterPrecise : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise = !_coll.gameObject.GetComponent<Mario>().bSlamAfterPrecise;
	}
}
