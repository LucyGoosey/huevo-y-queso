using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxDashes : MonoBehaviour
{
int maxDashes = 2;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxDashes = maxDashes;
	}
}
