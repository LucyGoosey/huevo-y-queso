using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMinSlideFloatSpeed : MonoBehaviour
{
float minSlideFloatSpeed = 0.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().minSlideFloatSpeed = minSlideFloatSpeed;
	}
}
