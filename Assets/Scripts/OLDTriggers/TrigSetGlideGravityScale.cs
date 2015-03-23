using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrigSetGlideGravityScale : MonoBehaviour
{
float glideGravityScale = 0.5f;

	void OnTriggerEnter2D(Collider2D _coll)
	{
		if(_coll.tag == "Player")
			_coll.gameObject.GetComponent<Mario>().glideGravityScale = glideGravityScale;
	}
}
