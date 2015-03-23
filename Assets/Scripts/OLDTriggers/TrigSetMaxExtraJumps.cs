using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxExtraJumps : MonoBehaviour
{
int maxExtraJumps = 1;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxExtraJumps = maxExtraJumps;
	}
}
