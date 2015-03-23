using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimedTrigger : BaseTrigger
{
    public int milliseconds = 0;
    private float seconds = 0f;
    private List<Huevo> runningOn = new List<Huevo>();

    new void Start()
    {
        base.Start();

        seconds = milliseconds / 1000f;
    }

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
        {
            Huevo h = _coll.GetComponent<Huevo>();

            if(!runningOn.Contains(h))
                StartCoroutine(DelayedTrigger( _coll.GetComponent<Huevo>()));
        }
    }

    IEnumerator DelayedTrigger(Huevo _huevo)
    {
        runningOn.Add(_huevo);
        
        yield return new WaitForSeconds(seconds);

        OnTrigger(_huevo);
        runningOn.Remove(_huevo);
    }
}
