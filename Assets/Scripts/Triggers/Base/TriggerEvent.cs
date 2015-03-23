using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public void OnTrigger(Huevo _huevo)
    {
    #if UNITY_EDITOR
        Debug.LogError("This isn't an actual trigger event!\nOn: " + gameObject.name);
    #endif
    }

    public void OnDetrigger(Huevo _huevo)
    {
    #if UNITY_EDITOR
        Debug.LogError("This isn't an actual trigger event!\nOn: " + gameObject.name);
    #endif
    }
}
