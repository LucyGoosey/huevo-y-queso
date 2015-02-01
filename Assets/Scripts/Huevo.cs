using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(InputHandler))]
public class Huevo : MonoBehaviour
{
    private class StateManager
    {
        public bool bOnGround = false;
        public bool bNearWall = false;
    }

    #region Variables
    #region Private
    private Rect worldHitBox;

    private float groundDistanceCheck = 0.1f;
    private Vector2 linecastCount = new Vector2(5, 5);

    private Vector2 velocity = Vector2.zero;
    private float vDeltaTime = 0;

    private InputHandler inHandler;

    private StateManager stateMan = new StateManager();
    #endregion

    public Vector2 hitboxWidthHeight = new Vector2(1.6f, 1.6f);

    public int playerNum = 0;

    public Vector2 gravity = new Vector2(0f, -24f);
    public Vector2 maxSpeed = new Vector2(15f, 35f);

    public float accel = 40f;
    public float jumpForce = 9f;

    public float groundDragCof = 1f;
    [Range(0f, 1f)]
    public float groundDragMagic = 0.05f;
    public float airDragCof = 1f;
    [Range(0f, 1f)]
    public float airDragMagic = 0f;
    #endregion

    void Start()
    {
        inHandler = GetComponent<InputHandler>();
        inHandler.SetPlayerNum(playerNum);

        worldHitBox.width = hitboxWidthHeight.x;
        worldHitBox.height = hitboxWidthHeight.y;
    }

    #region FixedUpdate
    void FixedUpdate()
    {
        PhysicsCheck();

        transform.position += (Vector3)(velocity * vDeltaTime);
        worldHitBox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));

        CalculateVelocity();
    }

    private void PhysicsCheck()
    {
        // Check for collision with the ground
        GameObject ground = GroundCheck(0, -1);
        if (ground != null)
        {
            stateMan.bOnGround = true;

            if (transform.parent != ground.transform)
                transform.parent = ground.transform;
        }
        else
            stateMan.bOnGround = false;

        // Check for collision with the ceiling
        GroundCheck(0, 1);

        // Check for collision with the wall to the relative right of Huevo
        GameObject wall = GroundCheck(1, 0);
        if (wall != null && !stateMan.bOnGround)
        {
            stateMan.bNearWall = true;

            if(transform.parent != wall.transform)
                transform.parent = wall.transform;
        }

        // Check for collision with the wall to the relative left
        GameObject otherWall = GroundCheck(-1, 0);
        if (otherWall != null && !stateMan.bOnGround && !stateMan.bNearWall)
        {
            stateMan.bNearWall = true;
            if(transform.parent != otherWall.transform)
                transform.parent = otherWall.transform;
        }

        // No collision with a wall on either side?
        if (wall == null && otherWall == null)
            stateMan.bNearWall = false;

        // If we're not on the ground or near a wall, we shouldn't "stick" to anything
        if(!stateMan.bOnGround && !stateMan.bNearWall)
            transform.parent = null;
    }

    private void CalculateVelocity()
    {
        vDeltaTime = Time.deltaTime;
        velocity += gravity * vDeltaTime;

        // Check for horizontal input, and apply acceleration if necessary
        if (inHandler.Horizontal != 0f)
            velocity.x += accel * inHandler.Horizontal * vDeltaTime * (stateMan.bOnGround ? groundDragCof : 1f);
        else if (stateMan.bOnGround)
            AddHorizontalDrag(groundDragMagic, groundDragCof);
        else if (!stateMan.bNearWall)
            AddHorizontalDrag(airDragMagic, airDragCof);

        // Limit the velocity to the max speed
        if (Mathf.Abs(velocity.x) > maxSpeed.x)
            velocity.x = maxSpeed.x * Mathf.Sign(velocity.x);
        if (Mathf.Abs(velocity.y) > maxSpeed.y)
            velocity.y = maxSpeed.y * Mathf.Sign(velocity.y);
    }

    private GameObject GroundCheck(int _xDir, int _yDir)
    {
        if (_xDir != 0 && _yDir != 0 || _xDir == 0 && _yDir == 0)
        {
            #if UNITY_DEBUG
            Debug.LogWarning("Invalid parameters passed to Huevo.GroundCheck().\nOnly x XOR y can have a value != 0.");
            #endif
            return null;
        }

        Rect testHitbox = worldHitBox;
        testHitbox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));
        testHitbox.center += velocity * Time.deltaTime;

        Vector2 start = new Vector2(_xDir == 0 ? testHitbox.xMin : testHitbox.center.x, _yDir == 0 ? testHitbox.yMin : testHitbox.center.y);
        float xD = _xDir == 0 ? testHitbox.width / (linecastCount.x + 1) : (testHitbox.width / 2f) + groundDistanceCheck;
        float yD = _yDir == 0 ? testHitbox.height / (linecastCount.y + 1) : (testHitbox.height / 2f) + groundDistanceCheck;

        if (_xDir == 0)
            start.x += xD;
        else if(_yDir == 0)
            start.y += yD;

        RaycastHit2D groundHit = new RaycastHit2D();
        int center = (int)(_xDir != 0 ? Mathf.Floor(linecastCount.x / 2) : Mathf.Floor(linecastCount.y / 2));
        for (int i = 0; i < (_xDir != 0 ? linecastCount.x : linecastCount.y); ++i)
        {
            int index = center + (i % 2 == 0 ? i / 2 : -((i / 2) + 1));

            Vector2 s = _xDir != 0 ? start + new Vector2(0f, (yD * index)) : start + new Vector2((xD * index), 0f);
            Vector2 e = Vector2.zero;
            if (_xDir != 0)
                e = s + new Vector2(xD * _xDir, 0f);
            else if(_yDir != 0)
                e = s + new Vector2(0f, yD * _yDir);

            groundHit = Physics2D.Linecast(s, e, 1 << LayerMask.NameToLayer("Ground"));

            #if UNITY_DEBUG
            if (groundHit)
            {
                Debug.DrawLine(s, e, Color.red);
                break;
            }else
                Debug.DrawLine(s, e);
            #else
            if (groundHit)
                break;
            #endif
        }

        if (groundHit)
        {
            if (_xDir != 0)
                if (Mathf.Sign(velocity.x) == _xDir)
                {
                    velocity.x = 0;
                    transform.position = new Vector3(_xDir > 0 ? groundHit.collider.bounds.min.x - (hitboxWidthHeight.x / 2) 
                                                                : groundHit.collider.bounds.max.x + (hitboxWidthHeight.x / 2),
                                                        transform.position.y);
                }
            if (_yDir != 0)
                if (Mathf.Sign(velocity.y) == _yDir)
                {
                    velocity.y = 0;
                    transform.position = new Vector3(transform.position.x, _yDir > 0 ? groundHit.collider.bounds.min.y - hitboxWidthHeight.y : groundHit.collider.bounds.max.y);
                }

            return groundHit.collider.gameObject;
        }

        return null;
    }

    private void AddHorizontalDrag(float _dragMagic, float _dragCof = 1f)
    {
        if (Mathf.Abs(velocity.x) > maxSpeed.x * 0.05f)
        {
            float vX = velocity.x;
            // Magic be here
            vX -= vX * (_dragMagic * Mathf.Pow(_dragCof, 2));

            if (Mathf.Sign(vX) != Mathf.Sign(velocity.x))
                velocity.x = 0f;
            else
                velocity.x = vX;
        }
        else
            velocity.x = 0f;
    }
    #endregion

    #region Update
    void Update()
    {
        if (inHandler.Jump.bDown)
            velocity.y = jumpForce;
    }
    #endregion

    #region UnityEditor
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.25f);
        worldHitBox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));

        if (worldHitBox.width != hitboxWidthHeight.x)
            worldHitBox.width = hitboxWidthHeight.x;
        if (worldHitBox.height != hitboxWidthHeight.y)
            worldHitBox.height = hitboxWidthHeight.y;

        // Draw worldHitbox
        Gizmos.DrawLine(new Vector3(worldHitBox.xMin, worldHitBox.yMin), new Vector3(worldHitBox.xMax, worldHitBox.yMin));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMax, worldHitBox.yMin), new Vector3(worldHitBox.xMax, worldHitBox.yMax));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMax, worldHitBox.yMax), new Vector3(worldHitBox.xMin, worldHitBox.yMax));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMin, worldHitBox.yMax), new Vector3(worldHitBox.xMin, worldHitBox.yMin));

        Vector2 start = new Vector2(worldHitBox.xMin, worldHitBox.yMin);
        float xD = worldHitBox.width / (linecastCount.x + 1);
        float yD = worldHitBox.height / (linecastCount.y + 1);

        start.x += xD;
        for (int i = 0; i < linecastCount.x; ++i, start.x += xD)
            Gizmos.DrawLine(start, start - new Vector2(0f, groundDistanceCheck));

        start = new Vector2(worldHitBox.xMin, worldHitBox.yMax);
        start.x += xD;
        for (int i = 0; i < linecastCount.x; ++i, start.x += xD)
            Gizmos.DrawLine(start, start + new Vector2(0f, groundDistanceCheck));

        start = new Vector2(worldHitBox.xMin, worldHitBox.yMin);
        start.y += yD;
        for (int i = 0; i < linecastCount.y; ++i, start.y += yD)
            Gizmos.DrawLine(start, start - new Vector2(groundDistanceCheck, 0f));

        start = new Vector2(worldHitBox.xMax, worldHitBox.yMin);
        start.y += yD;
        for (int i = 0; i < linecastCount.y; ++i, start.y += yD)
            Gizmos.DrawLine(start, start + new Vector2(groundDistanceCheck, 0f));
    }
#endif
    #endregion
}

