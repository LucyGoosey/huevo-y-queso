using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour {

    private bool bIsDead = false;
    public float bounceForce = 500f;

    public bool IsDead() { return bIsDead; }

    public void OnKill()
    {
        rigidbody2D.isKinematic = false;
        rigidbody2D.AddForce(Vector2.up * 200f);

        collider2D.enabled = false;
        bIsDead = true;
    }

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "PlayerFeet")
        {
            if (_coll.gameObject.transform.position.y + (_coll.gameObject.collider2D.bounds.size.y / 2) > transform.position.y + (collider2D.bounds.size.y / 2))
            {
                Mario m = _coll.transform.parent.GetComponent<Mario>();

                if (m.rigidbody2D.velocity.y < 0)
                {
                    OnKill();
                    m.rigidbody2D.AddForce(new Vector2(0, bounceForce));
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D _coll)
    {
        if (_coll.gameObject.tag == "Player" && !bIsDead)
        {
            Mario m = _coll.gameObject.GetComponent<Mario>();

            if (!m.IsDead())
                m.OnKill();
        }

    }
 }
