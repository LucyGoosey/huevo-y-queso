using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetMaxWallHangTime : MonoBehaviour
{
float maxWallHangTime = 2.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().maxWallHangTime = maxWallHangTime;
	}
}
