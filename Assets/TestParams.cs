namespace MarioTests
{
    public enum TestName
    {
        T_Jump,
        T_None
    }

    public enum TestState
    {
        TS_None,
        TS_End,
        JT_Accelerate,
        JT_Hold,
        JT_DoubleJump,
        JT_Glide,
        JT_MarkHigh,
        JT_Land
    }

    #region TestParameters
    public class Params
    {
        public bool bMarkLocations = false;
        public bool bAutoWallKick = false;
    }

    public class JTParams : Params
    {
        public bool bGoRight = true;
        public bool bHoldDir = false;
        public bool bLongJump = false;
        public bool bDoubleJump = false;
        public bool bGlide = false;
    }
    #endregion
}