using UnityEngine;

public class TriggerEffect : MonoBehaviour
{
    public void OnEffect(Huevo _huevo)
    {
#if UNITY_EDITOR
        Debug.LogWarning("This isn't actually a trigger effect!\nOn: " + gameObject.name);
#endif
    }
}
