using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxPreciseJumps : MonoBehaviour
{
int maxPreciseJumps = 1;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxPreciseJumps = maxPreciseJumps;
	}
}
