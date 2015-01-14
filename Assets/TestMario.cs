using UnityEngine;
using System.Collections;

using MarioTests;

public class TestMario : MonoBehaviour
{
    Mario linkedMario;

    TestState[] firstStates = new TestState[1] { TestState.JT_Accelerate };

    GameObject markerPrefab;
    GameObject specialMarkerPrefab;

    TestName curTest = TestName.T_None;
    TestState curState = TestState.TS_None;
    Params[] testParameters = new Params[1];
    Params curParams = null;
    public Params passiveParams = new Params();

    Vector2 lastLocationMarked = Vector2.zero;
    float minMarkerDistance = 1f;

    GameObject markerParent = null;
    Vector2 markerOffset = new Vector2(-0.8f, -0.8f);

    public GameObject selectedPath;
    new private Rigidbody2D rigidbody2D;

    #region VariableAccessors
    public TestName GetCurTest() { return curTest; }
    public TestState GetCurState() { return curState; }

    public Params GetTestParameters(TestName _test) { return testParameters[(int)_test]; }
    #endregion

    public void LinkMario(Mario _m) { linkedMario = _m; }

    public bool IsTesting() { return curTest != TestName.T_None && curState != TestState.TS_None; }

    void Start()
    {
        GameObject.FindObjectOfType<TestMarioGUI>().SetTestMario(this);
        rigidbody2D = transform.parent.rigidbody2D;

        transform.Find("TestMarioGUI").transform.SetParent(null, false);

        markerPrefab = Resources.Load<GameObject>("Marker");
        specialMarkerPrefab = Resources.Load<GameObject>("SpecialMarker");

        testParameters[0] = new JTParams();
        curParams = new Params();
    }

    #region TestControl

    public void BeginTest(TestName _test)
    {
        curTest = _test;
        curState = firstStates[(int)curTest];
        curParams = testParameters[(int)curTest];

        linkedMario.bUnderDirectControl = true;
    }

    public void EndTest()
    {
        curParams = new Params();
        curState = TestState.TS_None;
        curTest = TestName.T_None;

        linkedMario.bUnderDirectControl = false;
        if (!passiveParams.bMarkLocations)
            markerParent = null;
    }

    private void MarkLocation(bool _shouldMark, bool _specialMark = false)
    {
        if (_shouldMark && (Vector2.Distance(transform.position, lastLocationMarked) > minMarkerDistance || _specialMark))
        {
            if (markerParent == null)
            {
                markerParent = new GameObject("Marked Path");
                markerParent.transform.position = transform.position + (Vector3)markerOffset;

                GameObject mPos = new GameObject("Mario pos");
                mPos.transform.position = transform.position;
                mPos.transform.parent = markerParent.transform;
            }

            GameObject mark = (GameObject)Instantiate(_specialMark ? specialMarkerPrefab : markerPrefab, transform.position, Quaternion.identity);
            mark.transform.parent = markerParent.transform;
            lastLocationMarked = transform.position;
        }
    }
    #endregion

    #region Tests
    void FixedUpdate()
    {
        if (markerParent != null
            && (!passiveParams.bMarkLocations && curTest == TestName.T_None && curState == TestState.TS_None))
            markerParent = null;

        MarkLocation(passiveParams.bMarkLocations
                    || (!passiveParams.bMarkLocations && curParams.bMarkLocations
                        && curTest != TestName.T_None && curState != TestState.TS_None));

        if (linkedMario.IsDead() && curState != TestState.TS_None)
            curState = TestState.TS_End;

        if (curTest == TestName.T_None)
            return;

        if (curState == TestState.TS_None)
        {
            curTest = TestName.T_None;
            return;
        }

        switch (curState)
        {
            default:
                return;

            case TestState.JT_Accelerate:
                JTAccelerate();
                break;

            case TestState.JT_Hold:
                JTHold();
                break;

            case TestState.JT_DoubleJump:
                JTDoubleJump();
                break;

            case TestState.JT_Glide:
                JTGlide();
                break;

            case TestState.JT_Land:
                JTLand();
                break;

            case TestState.JT_MarkHigh:
                JTMarkHigh();
                break;

            case TestState.TS_End:
                EndTest();
                break;
        }
    }

    #region JumpTest

    void JTAccelerate()
    {
        bool bGoRight = ((JTParams)curParams).bGoRight;

        linkedMario.HandleMovementAlive(bGoRight ? 1f : -1f);

        bool bLongJump = ((JTParams)curParams).bLongJump;
        bool bDoubleJump = ((JTParams)curParams).bDoubleJump;
        bool bGlide = ((JTParams)curParams).bGlide;
        if (Mathf.Abs(rigidbody2D.velocity.x) >= linkedMario.maxSpeed.x)
        {
            if (passiveParams.bMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            curState = bLongJump ? TestState.JT_Hold : bDoubleJump ? TestState.JT_DoubleJump : bGlide ? TestState.JT_Glide : TestState.JT_MarkHigh;

            linkedMario.HandleJump(true);
            if (curState != TestState.JT_Hold)
                linkedMario.HandleJump(false);
        }
    }

    void JTHold()
    {
        if (Time.time - linkedMario.jumpHeldTime < linkedMario.longJumpTime)
        {
            linkedMario.HandleJump(true);
        }
        else
        {
            linkedMario.HandleJump(false);

            bool bDoubleJump = ((JTParams)curParams).bDoubleJump;
            bool bGlide = ((JTParams)curParams).bGlide;
            curState = bDoubleJump ? TestState.JT_DoubleJump : bGlide ? TestState.JT_Glide : TestState.JT_MarkHigh;
        }

        if (((JTParams)curParams).bHoldDir)
        {
            bool bGoRight = ((JTParams)curParams).bGoRight;

            linkedMario.HandleMovementAlive(bGoRight ? 1f : -1f);
        }
    }

    void JTDoubleJump()
    {
        if (rigidbody2D.velocity.y <= 0)
        {
            if (passiveParams.bMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            bool bGlide = ((JTParams)curParams).bGlide;
            curState = bGlide ? TestState.JT_Glide : TestState.JT_MarkHigh;

            linkedMario.HandleJump(true);
        }

        if (((JTParams)curParams).bHoldDir)
        {
            bool bGoRight = ((JTParams)curParams).bGoRight;

            linkedMario.HandleMovementAlive(bGoRight ? 1f : -1f);
        }
    }

    bool bGliding = false;
    void JTGlide()
    {
        if (rigidbody2D.velocity.y <= 0 || bGliding)
        {
            if (!bGliding)
                if (passiveParams.bMarkLocations || curParams.bMarkLocations)
                    MarkLocation(true, true);

            linkedMario.HandleGlide(true);
            bGliding = true;
        }
    }

    void JTMarkHigh()
    {
        if (rigidbody2D.velocity.y <= 0)
        {
            if (passiveParams.bMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            curState = TestState.JT_Land;
        }
    }

    void JTLand()
    {
        if (linkedMario.IsGrounded())
        {
            if (passiveParams.bMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            curState = TestState.TS_End;
        }

        if (((JTParams)curParams).bHoldDir)
            linkedMario.HandleMovementAlive(((JTParams)curParams).bGoRight ? 1f : -1f);
    }

    #endregion
    #endregion
}