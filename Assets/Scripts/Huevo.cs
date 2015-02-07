﻿using UnityEngine;
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
        public bool bHangingToWall = false;
        public bool bIsDashing = false;
    }

    #region Variables
    #region Private
    private Rect worldHitBox;

    private InputHandler inHandler;
    private StateManager stateMan = new StateManager();

    private float   groundDistanceCheck = 0.1f;
    private Vector2 linecastCount = new Vector2(5, 5);

    private Vector2 velocity = Vector2.zero;
    private float   vDeltaTime = 0;

    private bool    bLeftGround = false;
    private int     leftGroundForFrames = 0;

    private bool    bWantsToJump = false;
    private bool    bBlockJump = false;
    private bool    bLongJumping = false;
    private int     wallKickInputBlock = 0;
    private int     extraJumps = 0;
    private float   heldJumpFor = 0;
    private int     wallSide = 0;

    private Vector2 preHangGravity = Vector2.zero;
    private float   timeOnWall = 0f;

    private int     dashCombo = 0;
    private float   maxDashTime;
    private float   timeInDash = 0f;
    private float   dashDir = 0f;
    #endregion

    public Vector2 hitboxWidthHeight = new Vector2(1.6f, 1.6f);

    public int playerNum = 0;

    public Vector2 gravity = new Vector2(0f, -24f);
    public Vector2 maxSpeed = new Vector2(15f, 35f);

    public float accel = 40f;
    public float airAccelMod = 0.5f;
    public float reverseAccelMod = 1.75f;
    public float jumpForce = 10f;
    public int   maxExtraJumps = 1;
    public int   framesBeforeLeaveGround = 3;
    public int   framesToForgiveJump = 3;
    public float longJumpForce = 0.1f;
    public float maxLongJumpTime = 0.5f;

    public float groundDragCof = 1f;
    [Range(0f, 1f)]
    public float groundDragMagic = 0.05f;
    public float airDragCof = 1f;
    [Range(0f, 1f)]
    public float airDragMagic = 0f;

    public Vector2 wallKickForce = new Vector2(10f, 9f);
    public float maxWallHangTime = 1.5f;
    [Range(0f, 0.2f)]
    public float wallGrindMod = 0.075f;

    public float maxDashes = 2;
    public float dashVelocity = 15f;
    public float dashPauseTime = 0.1f;
    public float dashMotionTime = 0.3f;
    public float dashOverflowTime = 0.2f;
    #endregion

    void Start()
    {
        inHandler = GetComponent<InputHandler>();
        inHandler.SetPlayerNum(playerNum);

        worldHitBox.width = hitboxWidthHeight.x;
        worldHitBox.height = hitboxWidthHeight.y;

        maxDashTime = dashPauseTime + dashMotionTime + dashOverflowTime;
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
        Collider2D ground = GroundCheck(0, -1);
        if (ground != null)
        {
            if(velocity.y < 0f)
                velocity.y = 0f;
            transform.position = new Vector3(transform.position.x, ground.bounds.max.y);

            stateMan.bOnGround = true;
            Grounded();

            if (transform.parent != ground.transform)
                transform.parent = ground.transform;
        }
        else if (stateMan.bOnGround && !bLeftGround)
        {
            bLeftGround = true;
            leftGroundForFrames = 0;
        }

        // Check for collision with the ceiling
        Collider2D ceiling = GroundCheck(0, 1);
        if (ceiling != null)
        {
            if(velocity.y > 0f)
                velocity.y = 0f;

            transform.position = new Vector3(transform.position.x, ceiling.bounds.min.y - hitboxWidthHeight.y);
        }

        // Check for collision with the wall to the relative right of Huevo
        Collider2D wall = GroundCheck(1, 0);
        if (wall != null)
        {
            if(velocity.x > 0f)
                velocity.x = 0f;

            transform.position = new Vector3(wall.bounds.min.x - (hitboxWidthHeight.x / 2), transform.position.y);

            if (!stateMan.bOnGround)
            {
                stateMan.bNearWall = true;
                wallSide = 1;
                bBlockJump = false;

                if (transform.parent != wall.transform)
                    transform.parent = wall.transform;
            }
        }

        // Check for collision with the wall to the relative left
        Collider2D otherWall = GroundCheck(-1, 0);
        if (otherWall != null)
        {
            if(velocity.x < 0f)
                velocity.x = 0f;

            transform.position = new Vector3(otherWall.bounds.max.x + (hitboxWidthHeight.x / 2), transform.position.y);

            if (!stateMan.bOnGround && !stateMan.bNearWall)
            {
                stateMan.bNearWall = true;
                wallSide = -1;
                bBlockJump = false;

                if (transform.parent != otherWall.transform)
                    transform.parent = otherWall.transform;
            }
        }

        // No collision with a wall on either side?
        if (wall == null && otherWall == null)
            stateMan.bNearWall = false;

        // If we're not on the ground or near a wall, we shouldn't "stick" to anything
        if(!stateMan.bOnGround && !stateMan.bNearWall)
            transform.parent = null;
    }

    private void Grounded()
    {
        bLeftGround = false;
        bBlockJump = false;

        extraJumps = 0;

        wallKickInputBlock = 0;
        timeOnWall = 0;

        dashCombo = 0;
    }

    private void CalculateVelocity()
    {
        vDeltaTime = Time.deltaTime;

        if (IsBlockedByDash())
            return;

        velocity += gravity * vDeltaTime;

        // Check for horizontal input, and apply acceleration if necessary
        if (inHandler.Horizontal != 0f && wallKickInputBlock == 0)
        {
            float velAdd = accel * inHandler.Horizontal * vDeltaTime;

            if (!stateMan.bOnGround)
                velAdd *= airAccelMod;

            velAdd *= (stateMan.bOnGround ? groundDragCof : airDragCof);

            if (Mathf.Sign(velocity.x) != inHandler.Horizontal)
                velAdd *= reverseAccelMod;

            velocity.x += velAdd;
        }
        else if (stateMan.bOnGround)
            AddHorizontalDrag(groundDragMagic, groundDragCof);
        else if (!stateMan.bNearWall)
            AddHorizontalDrag(airDragMagic, airDragCof);

        // Limit the velocity to the max speed
        if (Mathf.Abs(velocity.x) > maxSpeed.x)
            velocity.x = maxSpeed.x * Mathf.Sign(velocity.x);
        if (!stateMan.bNearWall)
        {
            if (Mathf.Abs(velocity.y) > maxSpeed.y)
                velocity.y = maxSpeed.y * Mathf.Sign(velocity.y);
        }else
            if (Mathf.Abs(velocity.y) > maxSpeed.y * wallGrindMod)
                velocity.y = maxSpeed.y * Mathf.Sign(velocity.y) * wallGrindMod;
    }

    private bool IsBlockedByDash()
    {
        if (stateMan.bIsDashing)
        {
            if (timeInDash < dashPauseTime)
                velocity = Vector2.zero;
            else if (timeInDash - dashPauseTime < dashMotionTime)
            {
                if (GroundCheck((int)dashDir, 0, 2) != null)
                    timeInDash = maxDashTime;

                velocity = new Vector2(dashVelocity, 0f) * dashDir;
            }
            else
                return false;

            return true;
        }
        else
            return false;
    }

    private Collider2D GroundCheck(int _xDir, int _yDir, int framesToAdvance = 1)
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
        testHitbox.center += velocity * Time.deltaTime * framesToAdvance;

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
                return groundHit.collider;
            }else
                Debug.DrawLine(s, e);
            #else
            if (groundHit)
                return groundHit.collider;
            #endif
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
        if (wallKickInputBlock != (int)Mathf.Sign(inHandler.Horizontal))
            wallKickInputBlock = 0;

        if (stateMan.bIsDashing)
        {
            timeInDash += Time.deltaTime;

            if (timeInDash > maxDashTime)
            {
                stateMan.bIsDashing = false;
                inHandler.InputEnabled = true;
                timeInDash = 0f;
            }
        }

        HandleJump();
        HandleDash();

        if(!stateMan.bOnGround)
            HandleWallHang();

        CheckLeftGround();
    }

    private void HandleJump()
    {
        if (inHandler.Jump.bDown)
            CalcShouldWantJump();

        // If we can jump, and the jump button was just pressed...
        if (CanJump() && bWantsToJump)
        {
            bWantsToJump = false;

            if (stateMan.bNearWall)
            {
                velocity = wallKickForce;
                velocity.x *= -wallSide;

                stateMan.bNearWall = false;
                // Prevent any extra jumps after a wall kick
                extraJumps = maxExtraJumps;
                wallKickInputBlock = (int)Mathf.Sign(inHandler.Horizontal);
            }
            else
            {
                velocity.y = jumpForce;

                if (stateMan.bOnGround)
                {
                    stateMan.bOnGround = false;
                    bLongJumping = true;
                }
                else
                    ++extraJumps;
            }
        }
        else if (bLongJumping) // Otherwise, if we are long jumping
        {
            // Check if jump is being held
            if (inHandler.Jump.bHeld)
                velocity.y += longJumpForce; // And long jump if it is
            else
                bLongJumping = false;       // otherwise, stop long jumping

            heldJumpFor += Time.deltaTime;
            if (heldJumpFor > maxLongJumpTime)
                bLongJumping = false;

            if (!bLongJumping)
                heldJumpFor = 0;
        }
    }

    private void CalcShouldWantJump()
    {
        if (!stateMan.bOnGround && !stateMan.bNearWall)
        {
            bool flag = false;
            for (int i = 1; i < framesToForgiveJump; ++i)
                if (GroundCheck(0, -1, i) != null)
                {
                    flag = true;
                    bWantsToJump = true;
                    bBlockJump = true;
                    break;
                }

            for (int i = 1; i < framesToForgiveJump; ++i)
                if (GroundCheck(0, -1, i) != null)
                {
                    flag = true;
                    bWantsToJump = true;
                    bBlockJump = true;
                    break;
                }
                else if (GroundCheck(-1, 0, i) != null)
                {
                    flag = true;
                    bWantsToJump = true;
                    bBlockJump = true;
                    break;
                }

            if (!flag && extraJumps < maxExtraJumps)
                bWantsToJump = true;
        }
        else
            bWantsToJump = true;
    }

    private void HandleDash()
    {
        if (!stateMan.bIsDashing
            && inHandler.Dash != 0 && dashCombo < maxDashes)
        {
            dashDir = inHandler.Dash;
            stateMan.bIsDashing = true;
            inHandler.InputEnabled = false;
            ++dashCombo;
        }
    }

    private void HandleWallHang()
    {
        if (stateMan.bHangingToWall)
            timeOnWall += Time.deltaTime;
        
        if (!stateMan.bHangingToWall && stateMan.bNearWall 
            && inHandler.Horizontal == wallSide && timeOnWall <= maxWallHangTime)
        {
            preHangGravity = gravity;
            gravity = Vector2.zero;
            velocity = Vector2.zero;
            stateMan.bHangingToWall = true;
        }

        if (stateMan.bHangingToWall
            && (inHandler.Horizontal != wallSide 
                || !stateMan.bNearWall
                || timeOnWall > maxWallHangTime))
        {
            gravity = preHangGravity;
            stateMan.bHangingToWall = false;
        }
    }

    private void CheckLeftGround()
    {
        if (bLeftGround && stateMan.bOnGround)
        {
            if (leftGroundForFrames > framesBeforeLeaveGround)
                stateMan.bOnGround = false;

            ++leftGroundForFrames;
        }
    }

    private bool CanJump()
    {
        if (bBlockJump)
            return false;

        if (bLongJumping)
            return false;

        if (stateMan.bOnGround)
            return true;

        if (extraJumps < maxExtraJumps || stateMan.bNearWall)
            return true;

        return false;
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

