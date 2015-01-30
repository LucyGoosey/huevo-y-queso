using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSwitchGlideKillsY : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bGlideKillsY = !_coll.gameObject.GetComponent<Mario>().bGlideKillsY;
	}
}
