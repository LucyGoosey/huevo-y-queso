using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetLongWallKickForce : MonoBehaviour
{
float longWallKickForce = 5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().longWallKickForce = longWallKickForce;
	}
}
