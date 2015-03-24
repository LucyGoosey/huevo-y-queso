using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetGroundDragMagic : MonoBehaviour
{
float groundDragMagic = 0.05f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().groundDragMagic = groundDragMagic;
	}
}
