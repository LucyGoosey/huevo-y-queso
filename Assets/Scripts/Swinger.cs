using UnityEngine;
using System.Collections.Generic;

public class Swinger : Attachable {

    public float length = 3f;
    public Dictionary<Huevo, float> angVels = new Dictionary<Huevo, float>();

    void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i] != null)
            {
                Vector3 relPos = attached[i].transform.position - transform.position;
                float rad = Mathf.Atan2(relPos.y, relPos.x);
                angVels[attached[i]] += -24f * Mathf.Cos(rad) * Time.deltaTime;
                rad += angVels[attached[i]] * Time.deltaTime;
                AddDrag(ref rad, 10f, 0f, 0.025f, 1f);

                float x = length * Mathf.Cos(rad);
                float y = length * Mathf.Sin(rad);

                attached[i].transform.position += new Vector3((x - relPos.x), (y - relPos.y));

                attached[i].transform.position = transform.position + (attached[i].transform.position - transform.position).normalized * length;
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

    override protected void Detach(Huevo _h)
    {
        base.Detach(_h);
        if(angVels.ContainsKey(_h))
            angVels.Remove(_h);
    }
}
