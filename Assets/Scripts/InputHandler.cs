using UnityEngine;

class InputHandler : MonoBehaviour
{
    public class Key
    {
        public bool bDown = false;
        public bool bHeld = false;
        public bool bUp = false;
    }

    private bool bHandleInput = true;

    private int playerNum = 0;

    private Key jump = new Key();
    private Key slam = new Key();

    private float horizontal = 0f;
    private float vertical = 0f;
    private float dash = 0f;

    #region Accessors
    public bool InputEnabled{ get{return bHandleInput;} set{bHandleInput = value;} }

    public Key Jump { get { return (bHandleInput ? jump : new Key()); } }
    public Key Slam { get { return (bHandleInput ? slam : new Key()); } }

    public float Horizontal { get { return (bHandleInput ? horizontal : 0f); } }
    public float Vertical { get { return (bHandleInput ? vertical : 0f); } }
    public float Dash { get { return (bHandleInput ? dash : 0f); } }
    #endregion

    void Update()
    {
        if (playerNum == 0)
            return;

        HandleKey("Jump_" + playerNum, jump);
        HandleKey("Slam_" + playerNum, slam);

        HandleAxis("Horizontal_" + playerNum, ref horizontal);
        HandleAxis("Vertical_" + playerNum, ref vertical);
        HandleAxis("Dash_" + playerNum, ref dash);

        #region Debug
#if UNITY_DEBUG
        //LogKey("Jump", jump);
        //LogKey("Slam", jump);

        //LogAxis("Horizontal", horizontal);
        //LogAxis("Vertical", vertical);
        //LogAxis("Dash", dash);
#endif
        #endregion
    }

    private void HandleKey(string _keyName, Key _key)
    {
        if (_key.bUp)
            _key.bUp = false;

        if (Input.GetButton(_keyName) && !_key.bHeld && !_key.bDown)
            _key.bDown = true;
        else if (_key.bDown && Input.GetButton(_keyName))
        {
            _key.bDown = false;
            _key.bHeld = true;
        }
        else if ((_key.bHeld || _key.bDown) && !Input.GetButton(_keyName))
        {
            _key.bDown = false;
            _key.bHeld = false;
            _key.bUp = true;
        }
    }

    private void HandleAxis(string _axisName, ref float _axis)
    {
        _axis = Input.GetAxis(_axisName);

        if (_axis != 0f)
            _axis = _axis < 0f ? -1f : 1f;
    }

    #region Debug
#if UNITY_DEBUG
    private void LogKey(string _keyName, Key _key)
    {
        Debug.Log(_keyName + ":");
        Debug.Log("\tbDown: " + _key.bDown);
        Debug.Log("\tbHeld: " + _key.bHeld);
        Debug.Log("\tbUp: " + _key.bUp);
    }

    private void LogAxis(string _axisName, float _axis)
    {
        Debug.Log(_axisName + ": " + _axis);
    }
#endif
    #endregion

    public void SetPlayerNum(int _pNum)
    {
        // Player num is + 1 to make it string friendly
        playerNum = _pNum + 1;
    }
}
