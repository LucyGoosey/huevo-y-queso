using UnityEngine;
using System.Collections;

public class Swinger : Attachable {

    public float length = 3f;

    void FixedUpdate()
    {
        for (int i = 0; i < attached.Count; ++i)
            if (attached[i] != null)
            {
                Vector3 attPos = attached[i].transform.position + attached[i].GetComponent<Huevo>().HandPos;
                attached[i].transform.position = (transform.position + ((attPos - transform.position).normalized * length));
                attached[i].transform.position -= attached[i].GetComponent<Huevo>().HandPos;
            }
    }
}
