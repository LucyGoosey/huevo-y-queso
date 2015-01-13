using UnityEngine;
using System.Collections.Generic;

public class TestMario : Mario {

    enum TestName
    {
        T_Jump,
        T_None
    }

    enum TestState {
        TS_None,
        TS_End,
        JT_Accelerate,
        JT_Hold,
        JT_DoubleJump,
        JT_Glide,
        JT_Land
    }

    class Params
    {
        public TestState firstState;
        public bool bMarkLocations = false;
    }

    class JTParams : Params
    {
        new public TestState firstState = TestState.JT_Accelerate;
        public bool bGoRight = true;
        public bool bHoldDir = false;
        public bool bLongJump = false;
        public bool bDoubleJump = false;
        public bool bGlide = false;
    }

    public GameObject markerPrefab;

    TestName curTest = TestName.T_None;
    TestState curState = TestState.TS_None;
    Params[] testParameters = new Params[1];
    Params curParams = null;

    new void Start()
    {
        base.Start();

        testParameters[0] = new JTParams();
    }

    new void FixedUpdate()
    {
        if (IsDead() && curState != TestState.TS_None)
            curState = TestState.TS_End;

        if (curTest == TestName.T_None)
        {
            base.FixedUpdate();
            return;
        }

        if(curState == TestState.TS_None)
        {
            curTest = TestName.T_None;
            base.FixedUpdate();
            return;
        }

        if(curParams.bMarkLocations)
            Instantiate(markerPrefab, transform.position, Quaternion.identity);

        switch (curState)
        {
            default:
                base.FixedUpdate();
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

            case TestState.TS_End:
                EndTest();
                break;
        }
    }

    #region JumpTest

    void JTAccelerate()
    {
        bool bGoRight = ((JTParams)curParams).bGoRight;
        
        HandleMovementAlive(bGoRight ? 1f : -1f);
        
        bool bLongJump = ((JTParams)curParams).bLongJump;
        bool bDoubleJump = ((JTParams)curParams).bDoubleJump;
        bool bGlide = ((JTParams)curParams).bGlide;
        if (Mathf.Abs(rigidbody2D.velocity.x) >= maxSpeed.x)
        {
            curState = bLongJump ? TestState.JT_Hold : bDoubleJump ? TestState.JT_DoubleJump : bGlide ? TestState.JT_Glide : TestState.JT_Land;
            HandleJump(true);
            if (curState != TestState.JT_Hold)
                HandleJump(false);
        }
    }

    void JTHold()
    {
        if (Time.time - jumpHeldTime < longJumpTime)
        {
            HandleJump(true);
        }
        else
        {
            HandleJump(false);

            bool bDoubleJump = ((JTParams)curParams).bDoubleJump;
            bool bGlide = ((JTParams)curParams).bGlide;
            curState = bDoubleJump ? TestState.JT_DoubleJump : bGlide ? TestState.JT_Glide : TestState.JT_Land;
        }

        if (((JTParams)curParams).bHoldDir)
        {
            bool bGoRight = ((JTParams)curParams).bGoRight;
        
            HandleMovementAlive(bGoRight ? 1f : -1f);
        }
    }

    void JTDoubleJump()
    {
        if (rigidbody2D.velocity.y <= 0)
        {
            bool bGlide = ((JTParams)curParams).bGlide;
            curState = bGlide ? TestState.JT_Glide : TestState.JT_Land;

            HandleJump(true);
        }

        if (((JTParams)curParams).bHoldDir)
        {
            bool bGoRight = ((JTParams)curParams).bGoRight;
        
            HandleMovementAlive(bGoRight ? 1f : -1f);
        }
    }

    void JTGlide()
    {
        curState = TestState.JT_Land;
    }

    void JTLand()
    {
        if (IsGrounded())
        {
            curState = TestState.TS_End;
        }
        
        if (((JTParams)curParams).bHoldDir)
            HandleMovementAlive(((JTParams)curParams).bGoRight ? 1f : -1f);
    }

    #endregion

    #region GUI

    void OnGUI()
    {
        if (curState == TestState.TS_None)
            ShowTestOptions();
    }

    void ShowTestOptions()
    {
        TestName drawTest = TestName.T_None;
        {   // Jump test
            drawTest = TestName.T_Jump;
            JTParams parms = ((JTParams)testParameters[(int)drawTest]);

            parms.bMarkLocations = GUI.Toggle(new Rect(10, 35, 150, 20), ((JTParams)testParameters[(int)drawTest]).bMarkLocations, "Mark Locations?");
            parms.bGoRight = GUI.Toggle(new Rect(10, 60, 150, 20), ((JTParams)testParameters[(int)drawTest]).bGoRight, "Go Right?");
            parms.bHoldDir = GUI.Toggle(new Rect(10, 85, 150, 20), ((JTParams)testParameters[(int)drawTest]).bHoldDir, "Hold key?");
            parms.bLongJump = GUI.Toggle(new Rect(10, 110, 150, 20), ((JTParams)testParameters[(int)drawTest]).bLongJump, "Long Jump?");
            parms.bDoubleJump = GUI.Toggle(new Rect(10, 135, 150, 20), ((JTParams)testParameters[(int)drawTest]).bDoubleJump, "Double Jump?");
            parms.bGlide = GUI.Toggle(new Rect(10, 160, 150, 20), ((JTParams)testParameters[(int)drawTest]).bGlide, "Glide?");

            GUI.Label(new Rect(12.5f, 10, 70, 20), "Jump Test: ");
            if (GUI.Button(new Rect(80, 10, 60, 25), "Begin!"))
                BeginTest(drawTest, parms.firstState);
        }
    }

    #endregion

    void BeginTest(TestName _test, TestState _firstState)
    {
        curTest = _test;
        curState = _firstState;
        curParams = testParameters[(int)curTest];
    }

    void EndTest()
    {
        curParams = null;
        curState = TestState.TS_None;
        curTest = TestName.T_None;
    }
}
