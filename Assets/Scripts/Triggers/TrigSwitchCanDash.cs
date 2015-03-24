using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchCanDash : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bCanDash = !_coll.gameObject.GetComponent<Mario>().bCanDash;
	}
}
