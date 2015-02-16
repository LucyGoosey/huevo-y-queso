using UnityEngine;
using System.Collections.Generic;

public class Swinger : Attachable
{
    public class SwingingHuevo : AttachableHuevo
    {
        public float angVel = 0f;
        public float posOnVine = 0f;
        public float timeOnVine = 0f;

        public SwingingHuevo(Huevo _h) : base(_h) {}
    }

    public float length = 3f;
    private Dictionary<Huevo, float> angVels = new Dictionary<Huevo, float>();

    public bool ShouldApplyVelocity = true;
    public Vector2 jumpForce = new Vector2(0f, 10f);

    public float swingForce = 10f;
    public float gravity = -24f;

    public float maxSpeed = 3.5f;
    public float minSpeed = 0.5f;
    public float dragMagic = 0.025f;
    public float dragCof = 1f;

    public float timeToSlide = 0.5f;

    void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i] != null)
            {
                Huevo huevo = attached[i].huevo;
                Vector3 relPos = (huevo.transform.position + huevo.HandPos) - transform.position;
                float rad = Mathf.Atan2(relPos.y, relPos.x);

                if (huevo.InHandler.Horizontal != 0f && rad < -(Mathf.PI / 4f) && rad > -(Mathf.PI - (Mathf.PI / 4f)))
                    angVels[huevo] -= swingForce * huevo.InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime;
                else
                    angVels[huevo] += gravity * Mathf.Cos(rad) * Time.deltaTime;

                float dx, dy;
                dx = dy = 0f;
                if (angVels[huevo] != 0f)
                {
                    rad += angVels[huevo] * Time.deltaTime;
                    rad = rad > Mathf.PI ? -(Mathf.PI - (rad - Mathf.PI)) : rad < Mathf.PI ? Mathf.PI + (rad + Mathf.PI) : rad;
                    AddDrag(ref rad, maxSpeed, minSpeed, dragMagic, dragCof);

                    dx = (length * Mathf.Cos(rad)) - relPos.x;
                    dy = (length * Mathf.Sin(rad)) - relPos.y;

                    Debug.Log("dx: " + dx);
                    Debug.Log("dy: " + dy);

                    huevo.transform.position += new Vector3(dx, dy);
                }

                huevo.transform.position = transform.position + (((huevo.transform.position + huevo.HandPos) - transform.position).normalized * length);
                huevo.transform.position -= huevo.HandPos;

                if (huevo.InHandler.Jump.bDown)
                    Detach(huevo, dx, dy);
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
        SwingingHuevo sh = new SwingingHuevo(_h);
        if (!attached.Contains(sh))
        {
            angVels[_h] = 0f;
            sh.posOnVine = Vector2.Distance(transform.position, sh.huevo.transform.position);
            attached.Add(sh);
            sh.huevo.AttachToObject(this);
        }
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
