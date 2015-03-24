using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxGlideTime : MonoBehaviour
{
float maxGlideTime = 2f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxGlideTime = maxGlideTime;
	}
}
