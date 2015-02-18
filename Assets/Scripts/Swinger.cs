using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
#if UNITY_DEBUG
[ExecuteInEditMode]
#endif
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

    public Vector2 jumpForce = new Vector2(12f, 12f);

    public float swingForce = 30f;

    public float maxSpeed = 5f;
    private float minSpeed = 0f;
    public float dragMagic = 0.025f;
    public float dragCof = 1f;

    public float timeToSlide = 0.5f;

    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position - (Vector3)(Vector2.up * length));

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

        _sh.angVel += (huevo.EffectiveGravity.y / l) * Mathf.Cos(rad) * Time.deltaTime;
        _sh.angVel += (huevo.EffectiveGravity.x / l) * Mathf.Sin(rad) * Time.deltaTime;

        if (huevo.InHandler.Horizontal != 0f)
        {
            float sForce = swingForce / l;

            float pct = Mathf.Abs(rad);
            pct = pct > Mathf.PI / 2f ? (Mathf.PI / 2f) - (pct - (Mathf.PI / 2f)) : pct;
            pct /= Mathf.PI / 2f;
            sForce *= pct;

            _sh.angVel -= sForce * huevo.InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime;

            float dir = huevo.InHandler.Horizontal;
            if (rad < 0f)
                if (dir > 0 && rad < -(Mathf.PI / 2f))
                    _sh.angVel -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir > 0 && rad > -(Mathf.PI /2f))
                    _sh.angVel += sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir < 0 && rad > -(Mathf.PI / 2f))
                    _sh.angVel -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if(dir < 0 && rad < -(Mathf.PI / 2f))
                    _sh.angVel += sForce * Mathf.Cos(rad) * Time.deltaTime;
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

        lineRenderer.SetPosition(1, huevo.transform.position + huevo.HandPos);
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
        if (IsHuevoAttached(sh.huevo) == null && sh.huevo.transform.position.y + sh.huevo.HandPos.y < collider2D.bounds.max.y)
        {
            sh.posOnVine = Vector2.Distance(transform.position, sh.huevo.transform.position + sh.huevo.HandPos);
            Vector2 relPos = transform.position - sh.huevo.transform.position;
            Vector2 relPosPrime = transform.position - (sh.huevo.transform.position + (Vector3)sh.huevo.Velocity);
            sh.angVel = (Mathf.Atan2(relPos.y, relPosPrime.x) - Mathf.Atan2(relPos.y, relPos.x)) * sh.posOnVine;

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

#if UNITY_EDITOR
    new protected void Update()
    {
        if (!Application.isPlaying)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position - (Vector3)(Vector2.up * length));

            boxCollider.center = new Vector2(0f, -(length / 2f));
            boxCollider.size = new Vector2(0.25f, length);
        }
        else
            base.Update();
    }
#endif
}
