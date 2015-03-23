using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetGroundDragCof : MonoBehaviour
{
float groundDragCof = 1f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundDragCof = groundDragCof;
	}
}
