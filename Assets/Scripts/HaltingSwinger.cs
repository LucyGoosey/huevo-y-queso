﻿using UnityEngine;
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
                Huevo huevo = attached[i].huevo;
                Vector3 relPos = (huevo.transform.position + huevo.HandPos) - transform.position;
                float rad = Mathf.Atan2(relPos.y, relPos.x);

                float pct = Mathf.Abs(rad);
                pct = pct > Mathf.PI / 2f ? (Mathf.PI / 2f) - (pct - (Mathf.PI / 2f)) : pct;
                pct /= Mathf.PI / 2f;

                if (huevo.InHandler.Horizontal != 0f)
                    if (rad < -(Mathf.PI / 8f) && rad > -(Mathf.PI - (Mathf.PI / 8f)))
                        angVels[huevo] -= 10f * huevo.InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime * pct;
                    else
                        angVels[huevo] = 0f;
                else
                    angVels[huevo] += -24f * Mathf.Cos(rad) * Time.deltaTime;

                float dx, dy;
                dx = dy = 0f;
                if (angVels[huevo] != 0f)
                {
                    rad += angVels[huevo] * Time.deltaTime;
                    rad = rad > Mathf.PI ? -(Mathf.PI - (rad - Mathf.PI)) : rad < Mathf.PI ? Mathf.PI + (rad + Mathf.PI) : rad;
                    AddDrag(ref rad, 3.5f, 0.2f, 0.025f, 1f);

                    dx = (length * Mathf.Cos(rad)) - relPos.x;
                    dy = (length * Mathf.Sin(rad)) - relPos.y;

                    Debug.Log("dx: " + dx);
                    Debug.Log("dy: " + dy);

                    huevo.transform.position += new Vector3(dx, dy);
                }

                huevo.transform.position = transform.position + (((huevo.transform.position + huevo.HandPos) - transform.position).normalized * length);
                huevo.transform.position -= huevo.HandPos;

                if (huevo.InHandler.Jump.bDown)
                    Detach(attached[i--].huevo, dx, dy);
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