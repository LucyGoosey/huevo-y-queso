using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetTimeWallKickBlock : MonoBehaviour
{
float timeWallKickBlock = 0.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().timeWallKickBlock = timeWallKickBlock;
	}
}
