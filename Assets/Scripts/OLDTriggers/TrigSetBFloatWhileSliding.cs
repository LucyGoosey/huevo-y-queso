using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetBFloatWhileSliding : MonoBehaviour
{
bool bFloatWhileSliding = false;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bFloatWhileSliding = bFloatWhileSliding;
	}
}
