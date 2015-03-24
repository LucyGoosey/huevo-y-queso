using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetBSlideFloatForTime : MonoBehaviour
{
bool bSlideFloatForTime = true;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bSlideFloatForTime = bSlideFloatForTime;
	}
}
