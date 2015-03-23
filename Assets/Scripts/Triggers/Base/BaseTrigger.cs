using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BaseTrigger : MonoBehaviour 
{
    public bool oneShot = false;

    private TriggerEvent[] events = new TriggerEvent[0];

    protected void Start()
    {
        events = gameObject.GetComponents<TriggerEvent>();
    }

    protected void OnTrigger(Huevo _huevo)
    {
    #if UNITY_EDITOR
        if (events.Length == 0)
            Debug.LogWarning("No events attached to trigger: " + gameObject.name);
    #endif

        for (int i = 0; i < events.Length; ++i)
            events[i].OnTrigger(_huevo);

        if (oneShot)
            Physics2D.IgnoreCollision(collider2D, _huevo.collider2D);
    }

    protected void OnDetrigger(Huevo _huevo)
    {
    #if UNITY_EDITOR
        if (events.Length == 0)
            Debug.LogWarning("No events attached to trigger: " + gameObject.name);
    #endif

        for (int i = 0; i < events.Length; ++i)
            events[i].OnTrigger(_huevo);
    }
}
