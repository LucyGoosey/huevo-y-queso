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

    public bool ShouldApplyVelocity = true;
    public Vector2 jumpForce = new Vector2(0f, 10f);

    public float swingForce = 10f;
    public float gravity = -24f;

    public float maxSpeed = 5f;
    private float minSpeed = 0f;
    public float dragMagic = 0.025f;
    public float dragCof = 1f;

    public float timeToSlide = 0.5f;

    void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i].huevo != null)
                ProcessHuevo((SwingingHuevo)attached[i]);
    }

    private void ProcessHuevo(SwingingHuevo _sh)
    {
        Huevo huevo = _sh.huevo;

        Vector3 relPos = (huevo.transform.position + huevo.HandPos) - transform.position;
        float rad = Mathf.Atan2(relPos.y, relPos.x);

        float l = length;

        if (_sh.timeOnVine < timeToSlide)
        {
            l = _sh.posOnVine + (length - _sh.posOnVine) * (_sh.timeOnVine / timeToSlide);

            _sh.timeOnVine += Time.deltaTime;
        }

        _sh.angVel += (gravity / l) * Mathf.Cos(rad) * Time.deltaTime;

        if (huevo.InHandler.Horizontal != 0f)//&& rad < -(Mathf.PI / 4f) && rad > -(Mathf.PI - (Mathf.PI / 4f)))
        {
            float sForce = swingForce / l;
            _sh.angVel -= sForce * huevo.InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime;
            float dir = huevo.InHandler.Horizontal;
            if (rad < 0f)
            {
                if (dir > 0 && rad < -(Mathf.PI / 2f))
                    _sh.angVel -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir > 0 && rad > -(Mathf.PI /2f))
                    _sh.angVel += sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir < 0 && rad > -(Mathf.PI / 2f))
                    _sh.angVel -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if(dir < 0 && rad < -(Mathf.PI / 2f))
                    _sh.angVel += sForce * Mathf.Cos(rad) * Time.deltaTime;
            }
        }
        
        AddDrag(ref _sh.angVel, maxSpeed, minSpeed, dragMagic, dragCof);

        rad += (_sh.angVel * Time.deltaTime);

        Vector2 delta = Vector2.zero;

        delta.x = Mathf.Cos(rad) - relPos.x;
        delta.y = Mathf.Sin(rad) - relPos.y;

        huevo.transform.position += (Vector3)delta;

        huevo.transform.position = transform.position + (((huevo.transform.position + huevo.HandPos) - transform.position).normalized * l);    
        huevo.transform.position -= huevo.HandPos;

        if (huevo.InHandler.Jump.bDown || huevo.InHandler.Jump.bHeld)
            Detach(huevo);
    }

    private void AddDrag(ref float _out, float _maxSpeed, float _minSpeed, float _dragMagic, float _dragCof = 1f)
    {
        float vX = _out;
        // Magic be here
        vX -= vX * (_dragMagic * Mathf.Pow(_dragCof, 2));

        if (Mathf.Sign(vX) != Mathf.Sign(_out) || Mathf.Abs(vX) < _minSpeed)
            _out = 0f;
        else if (Mathf.Abs(vX) > _maxSpeed)
            _out = _maxSpeed * Mathf.Sign(vX);
        else
            _out = vX;
    }

    override protected void Attach(Huevo _h)
    {
        SwingingHuevo sh = new SwingingHuevo(_h);
        if (IsHuevoAttached(_h) == null)
        {
            sh.posOnVine = Vector2.Distance(transform.position, sh.huevo.transform.position + sh.huevo.HandPos);
            attached.Add(sh);
            sh.huevo.AttachToObject(this);
        }
    }

    override protected void Detach(Huevo _h)
    {
        SwingingHuevo sh = (SwingingHuevo)IsHuevoAttached(_h);
        if (sh != null)
        {
            base.Detach(_h);

            Vector3 relPos = (_h.transform.position + _h.HandPos) - transform.position;
            _h.AddForce(new Vector2(jumpForce.x * _h.Pawn.localScale.x, jumpForce.y));
        }
    }
}
