using UnityEngine;
using System.Collections;

public class TestMario : MonoBehaviour {

    Mario linkedMario;

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
        JT_MarkHigh,
        JT_Land
    }

    class Params
    {
        public bool bMarkLocations = false;
    }

    class JTParams : Params
    {
        public bool bGoRight = true;
        public bool bHoldDir = false;
        public bool bLongJump = false;
        public bool bDoubleJump = false;
        public bool bGlide = false;
    }

    TestState[] firstStates = new TestState[1] { TestState.JT_Accelerate };

    GameObject markerPrefab;
    GameObject specialMarkerPrefab;

    TestName curTest = TestName.T_None;
    TestState curState = TestState.TS_None;
    Params[] testParameters = new Params[1];
    Params curParams = null;

    bool bAlwaysMarkLocations = false;
    Vector2 lastLocationMarked = Vector2.zero;
    float minMarkerDistance = 1f;

    GameObject markerParent = null;
    Vector2 markerOffset = new Vector2(-0.8f, -0.8f);

    public GameObject selectedPath;

    void Start()
    {
        linkedMario = gameObject.GetComponent<Mario>();
        if (linkedMario == null)
        {
            Debug.Log("No mario linked to test mario!");
            Destroy(this);
        }

        markerPrefab = Resources.Load<GameObject>("Marker");
        specialMarkerPrefab = Resources.Load<GameObject>("SpecialMarker");

        testParameters[0] = new JTParams();
        curParams = new Params();
    }

    void FixedUpdate()
    {
        if (markerParent != null
            && (!bAlwaysMarkLocations && curTest == TestName.T_None && curState == TestState.TS_None))
            markerParent = null;

        MarkLocation(bAlwaysMarkLocations 
                    || (!bAlwaysMarkLocations && curParams.bMarkLocations 
                        && curTest != TestName.T_None && curState != TestState.TS_None));

        if (linkedMario.IsDead() && curState != TestState.TS_None)
            curState = TestState.TS_End;

        if (curTest == TestName.T_None)
            return;

        if(curState == TestState.TS_None)
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
            if (bAlwaysMarkLocations || curParams.bMarkLocations)
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
            if (bAlwaysMarkLocations || curParams.bMarkLocations)
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
            if(!bGliding)
                if (bAlwaysMarkLocations || curParams.bMarkLocations)
                    MarkLocation(true, true);

            linkedMario.HandleGlide(true);
            bGliding = true;
        }
    }

    void JTMarkHigh()
    {
        if (rigidbody2D.velocity.y <= 0)
        {
            if (bAlwaysMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            curState = TestState.JT_Land;
        }
    }

    void JTLand()
    {
        if (linkedMario.IsGrounded())
        {
            if (bAlwaysMarkLocations || curParams.bMarkLocations)
                MarkLocation(true, true);

            curState = TestState.TS_End;
        }
        
        if (((JTParams)curParams).bHoldDir)
            linkedMario.HandleMovementAlive(((JTParams)curParams).bGoRight ? 1f : -1f);
    }

    #endregion

    #region GUI

    void OnGUI()
    {
        if (curState == TestState.TS_None)
            ShowTestOptions();
        else
            if (GUI.Button(new Rect(10, 10, 150, 20), "End Test"))
                curState = TestState.TS_End;
    }

    void ShowTestOptions()
    {
        bAlwaysMarkLocations = GUI.Toggle(new Rect(150, 10, 160, 20), bAlwaysMarkLocations, "Always mark locations?");

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
                BeginTest(drawTest);
        }

        if (selectedPath != null && selectedPath.transform.Find("Mario pos") != null)
            if (GUI.Button(new Rect(320, 10, 150, 20), "Reset mario"))
                transform.position = selectedPath.transform.Find("Mario pos").position;
    }

    #endregion

    void BeginTest(TestName _test)
    {
        curTest = _test;
        curState = firstStates[(int)curTest];
        curParams = testParameters[(int)curTest];

        linkedMario.bUnderDirectControl = true;
    }

    void EndTest()
    {
        curParams = new Params();
        curState = TestState.TS_None;
        curTest = TestName.T_None;

        linkedMario.bUnderDirectControl = false;
        if(!bAlwaysMarkLocations)
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
}
