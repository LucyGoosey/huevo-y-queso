using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using MarioTests;

public class TestMarioGUI : MonoBehaviour {

    Toggle bMarkLocations;
    Toggle bAlwaysMarkLocations;

    public class JTGUIParams
    {
        public Toggle bGoRight;
        public Toggle bHoldDir ;
        public Toggle bLongJump ;
        public Toggle bDoubleJump ;
        public Toggle bGlide ;
    }

    private TestMario tMario;

    private bool bGUIEnabled = false;
    public bool GetGUIEnabled() { return bGUIEnabled; }
    public void SetGUIEnabled(bool _flag) { bGUIEnabled = _flag; }

    private bool bIsTesting = false;

    Toggle debugToggle;
    GameObject debugMenu;
    Button testButton;

    JTGUIParams jtParams = new JTGUIParams();

    public void SetTestMario(TestMario _tMario)
    {
        tMario = _tMario;
    }

    void Start()
    {
        debugToggle = GameObject.Find("DebugToggle").GetComponent<Toggle>();
        debugMenu = GameObject.Find("DebugGUI");

        testButton = GameObject.Find("DbgTest").GetComponent<Button>();
        bAlwaysMarkLocations = GameObject.Find("DbgMarkLocations").GetComponent<Toggle>();

        bMarkLocations = GameObject.Find("JTMarkLocations").GetComponent<Toggle>();
        jtParams.bGoRight = GameObject.Find("JTGoRight").GetComponent<Toggle>();
        jtParams.bHoldDir = GameObject.Find("JTHoldDir").GetComponent<Toggle>();
        jtParams.bLongJump = GameObject.Find("JTLongJump").GetComponent<Toggle>();
        jtParams.bDoubleJump = GameObject.Find("JTDoubleJump").GetComponent<Toggle>();
        jtParams.bGlide = GameObject.Find("JTGlide").GetComponent<Toggle>();
    }

    void Update()
    {
        if (debugMenu == null || debugToggle == null)
            return;

        tMario.passiveParams.bMarkLocations = bAlwaysMarkLocations.isOn;

        if (tMario.IsTesting() && !bIsTesting)
        {
            bIsTesting = true;
            testButton.GetComponentInChildren<Text>().text = "Stop";

            testButton.onClick.RemoveAllListeners();
            testButton.onClick.AddListener(tMario.EndTest);
        }
        else if (!tMario.IsTesting() && bIsTesting)
        {
            bIsTesting = false;
            testButton.GetComponentInChildren<Text>().text = "Start";

            testButton.onClick.RemoveAllListeners();
            testButton.onClick.AddListener(StartSelectedTest);
        }

        if (debugToggle.isOn && !debugMenu.activeSelf)
            debugMenu.SetActive(true);
        else if(!debugToggle.isOn && debugMenu.activeSelf)
            debugMenu.SetActive(false);
    }

    public void StartSelectedTest()
    {
        JTParams parms = ((JTParams)tMario.GetTestParameters(TestName.T_Jump));

        parms.bMarkLocations = bMarkLocations.isOn || bAlwaysMarkLocations.isOn;
        parms.bGoRight = jtParams.bGoRight.isOn;
        parms.bLongJump = jtParams.bLongJump.isOn;
        parms.bDoubleJump = jtParams.bDoubleJump.isOn;
        parms.bGlide = jtParams.bGlide.isOn;

        tMario.BeginTest(TestName.T_Jump);
    }

    #region GUI

    /*void OnGUI()
    {
        TestState curState = tMario.GetCurState();
        if (curState == TestState.TS_None)
            ShowTestOptions();
        else
            if (GUI.Button(new Rect(10, 10, 150, 20), "End Test"))
                curState = TestState.TS_End;
    }

    void ShowTestOptions()
    {
        tMario.SetAlwaysMarkLocations(GUI.Toggle(new Rect(150, 10, 160, 20), tMario.ShouldAlwaysMarkLocations(), "Always mark locations?"));

        TestName drawTest = TestName.T_None;
        {   // Jump test
            drawTest = TestName.T_Jump;
            JTParams parms = ((JTParams)tMario.GetTestParameters(drawTest));

            parms.bMarkLocations = GUI.Toggle(new Rect(10, 35, 150, 20), parms.bMarkLocations, "Mark Locations?");
            parms.bGoRight = GUI.Toggle(new Rect(10, 60, 150, 20), parms.bGoRight, "Go Right?");
            parms.bHoldDir = GUI.Toggle(new Rect(10, 85, 150, 20), parms.bHoldDir, "Hold key?");
            parms.bLongJump = GUI.Toggle(new Rect(10, 110, 150, 20), parms.bLongJump, "Long Jump?");
            parms.bDoubleJump = GUI.Toggle(new Rect(10, 135, 150, 20), parms.bDoubleJump, "Double Jump?");
            parms.bGlide = GUI.Toggle(new Rect(10, 160, 150, 20), parms.bGlide, "Glide?");

            GUI.Label(new Rect(12.5f, 10, 70, 20), "Jump Test: ");
            if (GUI.Button(new Rect(80, 10, 60, 25), "Begin!"))
                tMario.BeginTest(drawTest);
        }

        if (tMario.selectedPath != null && tMario.selectedPath.transform.Find("Mario pos") != null)
            if (GUI.Button(new Rect(320, 10, 150, 20), "Reset mario"))
                tMario.transform.position = tMario.selectedPath.transform.Find("Mario pos").position;
    }*/

    #endregion
}
