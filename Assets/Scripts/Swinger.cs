using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
#if UNITY_DEBUG
[ExecuteInEditMode]
#endif
public class Swinger : Attachable
{
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

    new protected void FixedUpdate()
    {
        base.FixedUpdate();

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position - (Vector3)(Vector2.up * length));

        for (int i = 0; i < attached.Count; ++i)
            if (attached[i].huevo != null)
                ProcessHuevo(attached[i]);
    }

    private void ProcessHuevo(AttachableHuevo _sh)
    {
        Huevo huevo = _sh.huevo;

        Vector3 relPos = (huevo.transform.position + huevo.HandPos) - transform.position;
        float rad = Mathf.Atan2(relPos.y, relPos.x);

        float l = length;

        if (_sh.floatParams[2] < timeToSlide)
        {
            l = _sh.floatParams[1] + (length - _sh.floatParams[1]) * (_sh.floatParams[3] / length);

            _sh.floatParams[3] = l;

            _sh.floatParams[2] += Time.deltaTime;
        }

        _sh.floatParams[0] += (huevo.EffectiveGravity.y / l) * Mathf.Cos(rad) * Time.deltaTime;
        _sh.floatParams[0] += (huevo.EffectiveGravity.x / l) * Mathf.Sin(rad) * Time.deltaTime;

        if (huevo.InHandler.Horizontal != 0f)
        {
            float sForce = swingForce / l;

            float pct = Mathf.Abs(rad);
            pct = pct > Mathf.PI / 2f ? (Mathf.PI / 2f) - (pct - (Mathf.PI / 2f)) : pct;
            pct /= Mathf.PI / 2f;
            sForce *= pct;

            _sh.floatParams[0] -= sForce * huevo.InHandler.Horizontal * Mathf.Sin(rad) * Time.deltaTime;

            float dir = huevo.InHandler.Horizontal;
            if (rad < 0f)
                if (dir > 0 && rad < -(Mathf.PI / 2f))
                    _sh.floatParams[0] -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir > 0 && rad > -(Mathf.PI /2f))
                    _sh.floatParams[0] += sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if (dir < 0 && rad > -(Mathf.PI / 2f))
                    _sh.floatParams[0] -= sForce * Mathf.Cos(rad) * Time.deltaTime;
                else if(dir < 0 && rad < -(Mathf.PI / 2f))
                    _sh.floatParams[0] += sForce * Mathf.Cos(rad) * Time.deltaTime;
        }
        
        AddDrag(ref _sh.floatParams[0], maxSpeed, dragMagic, dragCof);

        rad += (_sh.floatParams[0] * Time.deltaTime);

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

    private void AddDrag(ref float _out, float _maxSpeed, float _dragMagic, float _dragCof = 1f)
    {
        float vX = _out;
        vX -= Mathf.Sign(vX) * (_dragMagic * _dragCof);

        if (_out != 0 && Mathf.Sign(vX) != Mathf.Sign(_out))
            _out = 0f;
        else if (Mathf.Abs(vX) > _maxSpeed)
            _out = _maxSpeed * Mathf.Sign(vX);
        else
            _out = vX;
    }

    override protected AttachableHuevo Attach(Huevo _h)
    {
        if (_h.collider2D.bounds.max.y >= collider2D.bounds.max.y)
            return null;

        AttachableHuevo ah = base.Attach(_h);

        if (ah != null)
        {
            ah.SetNumFloatParams(4); // 0 = angVel, 1 = posOnVine, 2 = timeOnVine, 3 = curPosOnVine

            ah.floatParams[1] = Vector2.Distance(transform.position, ah.huevo.transform.position + ah.huevo.HandPos);
            ah.floatParams[3] = ah.floatParams[1];

            Vector2 relPos = transform.position - ah.huevo.transform.position;
            Vector2 relPosPrime = transform.position - (ah.huevo.transform.position + (Vector3)ah.huevo.Velocity);
            ah.floatParams[0] = (Mathf.Atan2(relPos.y, relPosPrime.x) - Mathf.Atan2(relPos.y, relPos.x)) * length;
        }

        return ah;
    }

    override protected void Detach(Huevo _h)
    {
        AttachableHuevo ah = IsHuevoAttached(_h);
        if (ah != null && !ah.bHoldingJump)
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
