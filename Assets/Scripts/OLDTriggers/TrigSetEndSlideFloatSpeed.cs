using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetEndSlideFloatSpeed : MonoBehaviour
{
float endSlideFloatSpeed = 0.2f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().endSlideFloatSpeed = endSlideFloatSpeed;
	}
}
