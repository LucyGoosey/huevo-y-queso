using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxSlideFloatTime : MonoBehaviour
{
float maxSlideFloatTime = 0.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSlideFloatTime = maxSlideFloatTime;
	}
}
