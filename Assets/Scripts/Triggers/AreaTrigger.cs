using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AreaTrigger : BaseTrigger
{
    private TriggerEffect[] effects = new TriggerEffect[0];

    new protected void Start()
    {
        base.Start();

        effects = gameObject.GetComponents<TriggerEffect>();
    }

    void OnTriggerEnter2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
        {
            OnTrigger(_coll.GetComponent<Huevo>());
            OnEffect(_coll.GetComponent<Huevo>());
        }
    }

    void OnTriggerStay2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
            OnEffect(_coll.GetComponent<Huevo>());
    }

    void OnTriggerExit2D(Collider2D _coll)
    {
        if (_coll.tag == "Player")
            OnDetrigger(_coll.GetComponent<Huevo>());
    }

    protected void OnEffect(Huevo _huevo)
    {
    #if UNITY_EDITOR
        if (effects.Length == 0)
            Debug.LogWarning("No effects attached to trigger: " + gameObject.name);
    #endif

        for (int i = 0; i < effects.Length; ++i)
            effects[i].OnEffect(_huevo);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AreaTrigger))]
public class AreaTriggerEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, new string[]{"oneShot"});
    }
}
#endif