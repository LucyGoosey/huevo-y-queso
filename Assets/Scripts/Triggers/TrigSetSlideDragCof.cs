using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetSlideDragCof : MonoBehaviour
{
float slideDragCof = 0.75f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().slideDragCof = slideDragCof;
	}
}
