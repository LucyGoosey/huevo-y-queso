using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxSlideTime : MonoBehaviour
{
float maxSlideTime = 1f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxSlideTime = maxSlideTime;
	}
}
