using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetBubblePopDistance : MonoBehaviour
{
float bubblePopDistance = 4f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().bubblePopDistance = bubblePopDistance;
	}
}
