using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetSlideFalloffTime : MonoBehaviour
{
float slideFalloffTime = 0.3f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().slideFalloffTime = slideFalloffTime;
	}
}
