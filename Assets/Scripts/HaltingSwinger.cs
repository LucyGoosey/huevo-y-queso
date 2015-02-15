using UnityEngine;
using System.Collections.Generic;

public class HaltingSwinger : Attachable {

    public float length = 3f;
    private Dictionary<Huevo, float> angVels = new Dictionary<Huevo, float>();

    public bool ShouldApplyVelocity = true;
    public Vector2 jumpForce = new Vector2(0f, 10f);

    void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i] != null)
            {
                Vector3 relPos = (attached[i].transform.position + attached[i].HandPos) - transform.position;
                float rad = Mathf.Atan2(relPos.y, relPos.x);

                float pct = Mathf.Abs(rad);
                pct = pct > Mathf.PI / 2f ? (Mathf.PI / 2f) - (pct - (Mathf.PI / 2f)) : pct;
                pct /= Mathf.PI / 2f;

                if (attached[i].InHandler.Horizontal != 0f)
                    if (rad < -(Mathf.PI / 4f) && rad > -(Mathf.PI - (Mathf.PI / 4f)))
                        angVels[attached[i]] -= 10f * attached[i].InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime * pct;
                    else
                        angVels[attached[i]] = 0f;
                else
                    angVels[attached[i]] += -24f * Mathf.Cos(rad) * Time.deltaTime;

                float dx, dy;
                dx = dy = 0f;
                if (angVels[attached[i]] != 0f)
                {
                    rad += angVels[attached[i]] * Time.deltaTime;
                    rad = rad > Mathf.PI ? -(Mathf.PI - (rad - Mathf.PI)) : rad < Mathf.PI ? Mathf.PI + (rad + Mathf.PI) : rad;
                    AddDrag(ref rad, 3.5f, 0.2f, 0.025f, 1f);

                    dx = (length * Mathf.Cos(rad)) - relPos.x;
                    dy = (length * Mathf.Sin(rad)) - relPos.y;

                    Debug.Log("dx: " + dx);
                    Debug.Log("dy: " + dy);

                    attached[i].transform.position += new Vector3(dx, dy);
                }

                attached[i].transform.position = transform.position + (((attached[i].transform.position + attached[i].HandPos) - transform.position).normalized * length);
                attached[i].transform.position -= attached[i].HandPos;

                if (attached[i].InHandler.Jump.bDown)
                    Detach(attached[i--], dx, dy);
            }
    }

    private void AddDrag(ref float _out, float _maxSpeed, float _minSpeed, float _dragMagic, float _dragCof = 1f)
    {
        float vX = _out;
        // Magic be here
        vX -= vX * (_dragMagic * Mathf.Pow(_dragCof, 2));

        if (Mathf.Sign(vX) != Mathf.Sign(_out) || Mathf.Abs(_out) < _minSpeed)
            _out = 0f;
        else
            _out = vX;
    }

    override protected void Attach(Huevo _h)
    {
        base.Attach(_h);
        if(!angVels.ContainsKey(_h))
            angVels.Add(_h, 0f);
    }

    protected void Detach(Huevo _h, float _dx = 0f, float _dy = 0f)
    {
        Vector3 relPos = (_h.transform.position + _h.HandPos) - transform.position;
        if (ShouldApplyVelocity)
            _h.AddForce(new Vector2((_dx / Time.deltaTime) + jumpForce.x, (_dy / Time.deltaTime) + jumpForce.y));
        else
            _h.AddForce(new Vector2(jumpForce.x * _h.Pawn.localScale.x, jumpForce.y));

        base.Detach(_h);

        if(angVels.ContainsKey(_h))
            angVels.Remove(_h);
    }
}
