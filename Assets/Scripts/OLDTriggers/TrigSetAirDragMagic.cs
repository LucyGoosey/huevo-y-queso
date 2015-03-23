using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetAirDragMagic : MonoBehaviour
{
float airDragMagic = 0f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().airDragMagic = airDragMagic;
	}
}
