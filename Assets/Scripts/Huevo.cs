using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Animator))]
public class Huevo : MonoBehaviour
{
    private class StateManager
    {
        public bool bIsAlive = false;
        public bool bOnGround = false;
        public bool bNearWall = false;
        public bool bHangingToWall = false;
        public bool bIsDashing = false;
        public bool bIsCrouching = false;
    }

    #region Private variables
    private Animator animator;
    private Transform pawn;
    public Transform Pawn { get { return pawn; } }
    
    private Rect worldHitBox;
    private Transform handPos;
    public Vector3 HandPos { get { return handPos.localPosition; } }

    private InputHandler inHandler;
    public InputHandler InHandler { get { return inHandler; } }
    private StateManager stateMan = new StateManager();

    private float   groundDistanceCheck = 0.1f;
    private Vector2 linecastCount = new Vector2(5, 5);

    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity { get { return velocity; } }
    private float   vDeltaTime = 0;

    private Vector2 effectiveGravity = new Vector2(0f, -24f);

    private bool    bLeftGround = false;
    private int     leftGroundForFrames = 0;

    private bool    bWantsToJump = false;
    private bool    bBlockJump = false;
    private bool    bLongJumping = false;
    private int     wallKickInputBlock = 0;
    private int     extraJumps = 0;
    private float   heldJumpFor = 0;
    private int     wallSide = 0;

    private float   timeOnWall = 0f;

    private int     dashCombo = 0;
    private float   maxDashTime;
    private float   timeInDash = 0f;
    private float   dashDir = 0f;

    private bool    bIsSlamming = false;

    private float   crouchFloatTime = 0f;

    private DeathBubble dBubble;
    private float   sqrDeadMaxSpeed;

    public Attachable attachedTo;

    private bool bIsBeingSquishedHor = false;
    private bool bIsBeingSquishedVert = false;
    #endregion

    #region Public Variables
    public Vector2 hitboxWidthHeight = new Vector2(1.6f, 1.6f);
    [Range(0f, 1f)]
    public float   wallHitBoxHeightPct = 0.51f;

    public int playerNum = 0;

    public Vector2 gravity = new Vector2(0f, -24f);
    public Vector2 maxSpeed = new Vector2(15f, 35f);
    public Vector2 minSpeed = new Vector2(0.05f, 0.01f);

    public float accel = 40f;
    public float airAccelMod = 0.5f;
    public float reverseAccelMod = 1.75f;
    public float jumpForce = 10f;
    public int   maxExtraJumps = 1;
    public int   framesBeforeLeaveGround = 3;
    public int   framesToForgiveJump = 3;
    public float longJumpForce = 0.17f;
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

    public float crouchSlideMinSpeed = 0.2f;
    public float crouchFloatMinSpeed = 0.5f;
    public float crouchFloatMaxTime = 0.4f;
    public float crouchFloatFalloffTime = 0.1f;

    public float deadAccel = 30f;
    public float deadMaxSpeed = 20f;
    public float deadDragMagic = 0.033f;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        pawn = transform.FindChild("Pawn");

        handPos = transform.FindChild("Hand");

        inHandler = GetComponent<InputHandler>();
        inHandler.SetPlayerNum(playerNum);

        worldHitBox.width = hitboxWidthHeight.x;
        worldHitBox.height = hitboxWidthHeight.y;

        maxDashTime = dashPauseTime + dashMotionTime + dashOverflowTime;

        sqrDeadMaxSpeed = Mathf.Pow(deadMaxSpeed, 2f);

        dBubble = gameObject.transform.FindChild("Death Bubble").GetComponent<DeathBubble>();

        stateMan.bIsAlive = true;
    }

    public void AddForce(Vector2 _force)
    {
        velocity += _force;
    }

    public void OnKill()
    {
        if (!stateMan.bIsAlive)
            return;

        transform.parent = null;
        velocity = Vector2.zero;
        stateMan = new StateManager();
        collider2D.enabled = false;
        dBubble.gameObject.SetActive(true);
        inHandler.InputEnabled = true;
        bIsBeingSquishedHor = bIsBeingSquishedVert = false;
    }

    public void OnBubblePop()
    {
        if (stateMan.bIsAlive)
            return;

        stateMan.bIsAlive = true;
        velocity /= 2f;
        collider2D.enabled = true;
        dBubble.gameObject.SetActive(false);
        effectiveGravity = gravity;
    }

    public void AttachToObject(Attachable _object)
    {
        transform.parent = _object.transform;
        attachedTo = _object;
        velocity = Vector2.zero;
    }

    public void DetachFromObject()
    {
        if (attachedTo != null)
        {
            transform.parent = null;
            attachedTo = null;
        }
    }

    #region FixedUpdate
    void FixedUpdate()
    {
        if (stateMan.bIsAlive)
            FixedUpdateAlive();
        else
            FixedUpdateDead();
    }

    void FixedUpdateAlive()
    {            
        CollisionCheck();

        if (attachedTo != null)
        {
            return;
        }

        rigidbody2D.MovePosition(transform.position + (Vector3)(velocity * vDeltaTime));
        worldHitBox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));

        CalculateVelocity();
    }

    #region Collision Check
    private void CollisionCheck()
    {
        Rect testHitbox = worldHitBox;
        testHitbox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));

        RaycastHit2D[] hits = new RaycastHit2D[4];
        for (int i = 0; i < 4; ++i)
            hits[i] = new RaycastHit2D();
        
        hits[0] = GroundCheck(0, -1);
        hits[1] = GroundCheck(0, 1);

        hits[2] = GroundCheck(1, 0);
        hits[3] = GroundCheck(-1, 0);

        if(hits[0].collider == null)
            if (stateMan.bOnGround && !bLeftGround)
            {
                bLeftGround = true;
                leftGroundForFrames = 0;
            }

        RaycastHit2D hor, ver;
        hor = new RaycastHit2D();
        ver = new RaycastHit2D();

        if (hits[0].collider != null)
            ver = hits[0];
        else if (hits[1].collider != null)
            ver = hits[1];

        if (hits[2].collider != null)
            hor = hits[2];
        else if (hits[3].collider != null)
            hor = hits[3];

        if (hor.collider != null && ver.collider != null && hor.collider == ver.collider)
        {
            float xD = Mathf.Min(Mathf.Abs(testHitbox.center.x - hor.collider.bounds.max.x), Mathf.Abs(testHitbox.center.x - hor.collider.bounds.min.x));
            float yD = Mathf.Min(Mathf.Abs(testHitbox.center.y - ver.collider.bounds.max.y), Mathf.Abs(testHitbox.center.y - ver.collider.bounds.min.y));

            if (xD < yD)
            {
                ver = new RaycastHit2D();
            }
            else
            {
                hor = new RaycastHit2D();
            }
        }

        if (ver.collider != null)
        {
            float upDist, downDist;
            upDist = downDist = float.MaxValue;

            upDist = Mathf.Abs(testHitbox.center.y - ver.collider.bounds.min.y);
            downDist = Mathf.Abs(testHitbox.center.y - ver.collider.bounds.max.y);

            float dir = upDist < downDist ? 1f : -1f;

            if (dir == -1f && !stateMan.bOnGround && velocity.y < 0f)
                Grounded(ver.collider.transform);

            if (Mathf.Sign(velocity.y) == dir)
                velocity.y = 0f;

            float newY = dir > 0f ? ver.collider.bounds.min.y - hitboxWidthHeight.y : ver.collider.bounds.max.y;
            transform.position = new Vector3(transform.position.x, newY);

            if (hits[0].collider != null && hits[1].collider != null)
            {
                if (bIsBeingSquishedVert)
                    OnKill();
                else
                    bIsBeingSquishedVert = true;
            }else
                bIsBeingSquishedVert = false;
        }

        // We're not near a wall unless we are near a wall!
        stateMan.bNearWall = false;

        if (hor.collider != null)
        {
            float leftDist, rightDist;
            leftDist = rightDist = float.MaxValue;

            leftDist = Mathf.Abs(testHitbox.center.x - hor.collider.bounds.max.x);
            rightDist = Mathf.Abs(testHitbox.center.x - hor.collider.bounds.min.x);

            float dir = leftDist < rightDist ? -1f : 1f;

            if (Mathf.Sign(velocity.x) == dir)
                velocity.x = 0f;

            float halfWidth = hitboxWidthHeight.x / 2f;
            float newX = dir > 0f ? hor.collider.bounds.min.x - halfWidth : hor.collider.bounds.max.x + halfWidth;
            transform.position = new Vector3(newX, transform.position.y);

            CheckNearWall(hor, dir);

            if (hits[2].collider != null && hits[3].collider != null)
            {
                if (bIsBeingSquishedHor)
                    OnKill();
                else
                    bIsBeingSquishedHor = true;
            }
            else
                bIsBeingSquishedHor = false;
        }

        // If we're not on the ground or near a wall, we shouldn't "stick" to anything
        if(!stateMan.bOnGround && !stateMan.bNearWall)
            transform.parent = null;
    }

    private RaycastHit2D IterateGroundCheck(float vel, float dist, Vector2 groundcheckDir)
    {
        // Usage from PhysicsCheck()
        /*if (velocity != Vector2.zero)
        {
            float mag = velocity.magnitude;

            float dist = testHitbox.height / (linecastCount.y + 1);
            hits[0] = IterateGroundCheck(mag, dist, new Vector2(0, -1)); // Check for collision with the ground
            hits[1] = IterateGroundCheck(mag, dist, new Vector2(0, 1));  // Check for collision with the ceiling

            dist = testHitbox.width / (linecastCount.x + 1);
            hits[2] = IterateGroundCheck(mag, dist, new Vector2(1, 0));  // Check for collision with the r-wall 
            hits[3] = IterateGroundCheck(mag, dist, new Vector2(-1, 0)); // Check for collision with the l-wall 
        }*/
        vel = Mathf.Abs(vel);
        if (vel > dist)
        {
            float iterations = Mathf.Ceil(vel / dist) / 2;
            float pct = (dist / vel) * 2;

            for (int i = 0; i < iterations; ++i)
            {
                RaycastHit2D hit = GroundCheck((int)groundcheckDir.x, (int)groundcheckDir.y, pct * i);
                if (hit.collider != null)
                    return hit;
            }
        }

        return new RaycastHit2D();
    }

    private RaycastHit2D GroundCheck(int _xDir, int _yDir, float framesToAdvance = 1f)
    {
        RaycastHit2D groundHit = new RaycastHit2D();

        if (_xDir != 0 && _yDir != 0 || _xDir == 0 && _yDir == 0)
        {
            #if UNITY_DEBUG
            Debug.LogWarning("Invalid parameters passed to Huevo.GroundCheck().\nOnly x XOR y can have a value != 0.");
            #endif
            return groundHit;
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

        int center = (int)(_xDir != 0 ? Mathf.Floor(linecastCount.x / 2) : Mathf.Floor(linecastCount.y / 2));
        for (int i = 0; i < (_xDir != 0 ? linecastCount.x : linecastCount.y); ++i)
        {
            int index = center + (i % 2 == 0 ? i / 2 : -((i / 2) + 1));

            Vector2 s = _xDir != 0 ? start + new Vector2(0f, (yD * index)) : start + new Vector2((xD * index), 0f);
            Vector2 e = Vector2.zero;
            if (_xDir != 0)
                e = s + new Vector2(xD * _xDir, 0f);
            else if (_yDir != 0)
                e = s + new Vector2(0f, yD * _yDir);

            groundHit = Physics2D.Linecast(s, e, 1 << LayerMask.NameToLayer("Ground"));

            #if UNITY_DEBUG
            if (groundHit)
            {
                Debug.DrawLine(s, e, Color.red);
                return groundHit;
            }else
                Debug.DrawLine(s, e);
            #else
            if (groundHit)
                return groundHit.collider;
            #endif
        }

        return groundHit;
    }

    private void CheckNearWall(RaycastHit2D _wall, float _dir)
    {
        if (_wall.point.y < worldHitBox.yMin + (hitboxWidthHeight.y * wallHitBoxHeightPct))
        {
            stateMan.bNearWall = true;
            wallSide = (int)_dir;

            if (!stateMan.bOnGround)
            {
                bBlockJump = false;

                if (transform.parent != _wall.collider.transform)
                    transform.parent = _wall.collider.transform;
            }
        }
    }

    private void Grounded(Transform _ground)
    {
        if (transform.parent != _ground)
            transform.parent = _ground;

        stateMan.bOnGround = true;
        bLeftGround = false;
        bBlockJump = false;

        extraJumps = 0;

        wallKickInputBlock = 0;
        timeOnWall = 0;

        dashCombo = 0;

        if (bIsSlamming)
        {
            bIsSlamming = false;
            inHandler.InputEnabled = true;
        }
    }
    #endregion

    private void CalculateVelocity()
    {
        vDeltaTime = Time.deltaTime;

        if (IsBlockedByDash())
            return;

        if(!bLongJumping)
            velocity += effectiveGravity * vDeltaTime;
        else // Otherwise, if we are long jumping
        {
            // Check if jump is being held
            float pct = (heldJumpFor / maxLongJumpTime);

            if (inHandler.Jump.bHeld && !ShouldWallGrind())
                velocity.y += effectiveGravity.y * pct * vDeltaTime; // And long jump if it is
            else
                bLongJumping = false;       // otherwise, stop long jumping

            heldJumpFor += Time.deltaTime;
            if (heldJumpFor > maxLongJumpTime)
                bLongJumping = false;

            if (!bLongJumping)
                heldJumpFor = 0;
        }

        GetHorizontalInput();

        if (stateMan.bOnGround && inHandler.Horizontal == 0f)
            AddDrag(ref velocity.x, maxSpeed.x, minSpeed.x, groundDragMagic, groundDragCof);
        else if (!stateMan.bNearWall)
        {
            AddDrag(ref velocity.x, maxSpeed.x, minSpeed.x, airDragMagic, airDragCof);
            AddDrag(ref velocity.y, maxSpeed.y, minSpeed.y, airDragMagic, airDragCof);
        }

        // Limit the velocity to the max speed
        if (Mathf.Abs(velocity.x) > maxSpeed.x)
            velocity.x = maxSpeed.x * Mathf.Sign(velocity.x);
        if (ShouldWallGrind())
        {
            if (Mathf.Abs(velocity.y) > maxSpeed.y * wallGrindMod)
                velocity.y = maxSpeed.y * Mathf.Sign(velocity.y) * wallGrindMod;
        }else
            if (Mathf.Abs(velocity.y) > maxSpeed.y)
                velocity.y = maxSpeed.y * Mathf.Sign(velocity.y);
    }

    private void GetHorizontalInput()
    {
        // Check for horizontal input, and apply acceleration if necessary
        if (inHandler.Horizontal != 0f && wallKickInputBlock == 0 && !stateMan.bIsCrouching)
        {
            float velAdd = accel * inHandler.Horizontal * vDeltaTime;

            if (!stateMan.bOnGround)
                velAdd *= airAccelMod;

            velAdd *= (stateMan.bOnGround ? groundDragCof : airDragCof);

            if (Mathf.Sign(velocity.x) != inHandler.Horizontal)
                velAdd *= reverseAccelMod;

            velocity.x += velAdd;
        }
    }

    private bool ShouldWallGrind()
    {
        return !stateMan.bOnGround && stateMan.bNearWall && !stateMan.bHangingToWall 
                && (pawn.transform.localScale.x == wallSide) && !bIsSlamming;
    }

    private bool IsBlockedByDash()
    {
        if (stateMan.bIsDashing)
        {
            if (timeInDash < dashPauseTime)
                velocity = Vector2.zero;
            else if (timeInDash - dashPauseTime < dashMotionTime)
            {
                if (GroundCheck((int)dashDir, 0, 2).collider != null)
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

    private void AddDrag(ref float _out, float _maxSpeed, float _minSpeed, float _dragMagic, float _dragCof = 1f)
    {
        float vX = _out;
        // Magic be here
        vX -= vX * (_dragMagic * Mathf.Pow(_dragCof, 2));

        if (Mathf.Sign(vX) != Mathf.Sign(_out) || Mathf.Abs(_out) < _minSpeed)
            _out = 0f;
        else
            _out = vX;
    }

    private void FixedUpdateDead()
    {
        velocity.x += deadAccel * inHandler.Horizontal * vDeltaTime;
        velocity.y += deadAccel * inHandler.Vertical * vDeltaTime;

        if (velocity.sqrMagnitude > sqrDeadMaxSpeed)
            velocity = velocity.normalized * deadMaxSpeed;

        if(inHandler.Horizontal == 0)
            AddDrag(ref velocity.x, deadMaxSpeed, 0f, deadDragMagic);
        if(inHandler.Vertical == 0)
            AddDrag(ref velocity.y, deadMaxSpeed, 0f, deadDragMagic);

        rigidbody2D.MovePosition(transform.position + (Vector3)(velocity * Time.deltaTime));
    }
    #endregion

    #region Update
    void Update()
    {
        if (stateMan.bIsAlive)
            UpdateAlive();
        else
            UpdateDead();

        UpdateAnimation();
    }

    private void UpdateAlive()
    {
        if(inHandler.Bubble.bDown)
        {
            OnKill();
            return;
        }

        if (attachedTo != null)
        {

            return;
        }

        if (wallKickInputBlock != (int)Mathf.Sign(inHandler.Horizontal))
            wallKickInputBlock = 0;

        HandleJump();
        HandleDash();

        if (!stateMan.bOnGround)
            HandleWallHang();

        HandleSlam();

        HandleCrouch();

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
            if(stateMan.bIsCrouching)
                EndCrouch();

            if (stateMan.bNearWall && !stateMan.bOnGround)
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
                    bLeftGround = true;
                    leftGroundForFrames = int.MaxValue;
                    stateMan.bOnGround = false;
                    bLongJumping = true;
                }
                else
                    ++extraJumps;
            }
        }
    }

    private void CalcShouldWantJump()
    {
        if (!stateMan.bOnGround && !stateMan.bNearWall)
        {
            bool flag = false;
            for (int i = 1; i < framesToForgiveJump; ++i)
                if (GroundCheck(0, -1, i).collider != null)
                {
                    flag = true;
                    bWantsToJump = true;
                    bBlockJump = true;
                    break;
                }

            for (int i = 1; i < framesToForgiveJump; ++i)
                if (GroundCheck(0, -1, i).collider != null)
                {
                    flag = true;
                    bWantsToJump = true;
                    bBlockJump = true;
                    break;
                }
                else if (GroundCheck(-1, 0, i).collider != null)
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

        if (!stateMan.bIsDashing && inHandler.Dash != 0 
            && dashCombo < maxDashes
            && (!stateMan.bNearWall || (stateMan.bNearWall && inHandler.Dash != wallSide)))
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
            effectiveGravity = Vector2.zero;
            velocity = Vector2.zero;
            stateMan.bHangingToWall = true;
        }

        if (stateMan.bHangingToWall
            && (inHandler.Horizontal != wallSide 
                || !stateMan.bNearWall
                || timeOnWall > maxWallHangTime))
        {
            effectiveGravity = gravity;
            stateMan.bHangingToWall = false;
        }
    }

    private void HandleSlam()
    {
        if ((!stateMan.bOnGround || bLeftGround) && inHandler.Slam.bDown && !bIsSlamming)
        {
            bIsSlamming = true;
            inHandler.InputEnabled = false;
            velocity = new Vector2(0, -maxSpeed.y);
        }
    }

    private void HandleCrouch()
    {
        if (stateMan.bIsCrouching)
            if (stateMan.bOnGround && !bLeftGround)
            {
                effectiveGravity = gravity;
                crouchFloatTime = 0f;

                if (Mathf.Abs(velocity.x) < maxSpeed.x * crouchSlideMinSpeed)
                    velocity.x = 0f;
            }
            else
            {
                velocity.y = 0f;
                leftGroundForFrames = 0;
                
                effectiveGravity = new Vector2(0f, 0f);
                crouchFloatTime += Time.deltaTime;

                if (crouchFloatTime > crouchFloatMaxTime)
                {
                    float falloffTime = crouchFloatTime - crouchFloatMaxTime;
                    if (falloffTime < crouchFloatFalloffTime)
                        effectiveGravity = gravity * (falloffTime / crouchFloatFalloffTime);
                    else
                        EndCrouch();
                }
            }

        if (!stateMan.bIsCrouching && inHandler.Vertical == -1f && stateMan.bOnGround && !bLeftGround)
        {
            stateMan.bIsCrouching = true;

            if (Mathf.Abs(velocity.x) < maxSpeed.x * crouchSlideMinSpeed)
                velocity.x = 0f;
        }
        else if (stateMan.bIsCrouching && inHandler.Vertical != -1f)
            EndCrouch();
    }

    private void EndCrouch()
    {
        stateMan.bIsCrouching = false;
        effectiveGravity = gravity;
        crouchFloatTime = 0f;
    }

    private void CheckLeftGround()
    {
        if (bLeftGround && stateMan.bOnGround)
        {
            if (leftGroundForFrames >= framesBeforeLeaveGround)
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

    private void UpdateDead()
    {
        if (inHandler.Bubble.bDown)
            OnBubblePop();
    }

    private void UpdateAnimation()
    {
        animator.SetBool("bOnGround", stateMan.bOnGround);
        animator.SetBool("bShouldWallGrind", ShouldWallGrind());
        animator.SetBool("bHangingOnWall", stateMan.bHangingToWall);
        animator.SetBool("bIsDashing", stateMan.bIsDashing);
        animator.SetBool("bIsStill", Mathf.Abs(velocity.x) < maxSpeed.x * 0.05f);
        animator.SetBool("bIsCrouching", stateMan.bIsCrouching);

        if (inHandler.Horizontal != 0)
            if (Mathf.Sign(pawn.localScale.x) != inHandler.Horizontal)
                pawn.localScale = new Vector3(pawn.localScale.x * -1f, pawn.localScale.y, pawn.localScale.z);
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
        float yMin = worldHitBox.yMin + (worldHitBox.height * wallHitBoxHeightPct);
        Gizmos.DrawLine(new Vector3(worldHitBox.xMax, yMin), new Vector3(worldHitBox.xMax, worldHitBox.yMax));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMax, worldHitBox.yMax), new Vector3(worldHitBox.xMin, worldHitBox.yMax));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMin, worldHitBox.yMax), new Vector3(worldHitBox.xMin, yMin));

        // Draw wallHitbox
        Gizmos.color = new Color(0.75f, 0.66f, 0f);
        Gizmos.DrawLine(new Vector3(worldHitBox.xMin, worldHitBox.yMin), new Vector3(worldHitBox.xMax, worldHitBox.yMin));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMax, worldHitBox.yMin), new Vector3(worldHitBox.xMax, yMin));
        Gizmos.DrawLine(new Vector3(worldHitBox.xMin, yMin), new Vector3(worldHitBox.xMin, worldHitBox.yMin));

        Gizmos.color = new Color(0.8f, 0.2f, 0.25f);
        Vector2 start = new Vector2(worldHitBox.xMin, worldHitBox.yMin);
        float xD = worldHitBox.width / (linecastCount.x + 1);
        float yD = worldHitBox.height / (linecastCount.y + 1);

        Gizmos.color = new Color(0.75f, 0.66f, 0f);
        start.x += xD;
        for (int i = 0; i < linecastCount.x; ++i, start.x += xD)
            Gizmos.DrawLine(start, start - new Vector2(0f, groundDistanceCheck));

        Gizmos.color = new Color(0.8f, 0.2f, 0.25f);
        start = new Vector2(worldHitBox.xMin, worldHitBox.yMax);
        start.x += xD;
        for (int i = 0; i < linecastCount.x; ++i, start.x += xD)
            Gizmos.DrawLine(start, start + new Vector2(0f, groundDistanceCheck));

        start = new Vector2(worldHitBox.xMin, worldHitBox.yMin);
        start.y += yD;
        for (int i = 0; i < linecastCount.y; ++i, start.y += yD)
        {
            if (start.y < yMin)
                Gizmos.color = new Color(0.75f, 0.66f, 0f);
            else
                Gizmos.color = new Color(0.8f, 0.2f, 0.25f);

            Gizmos.DrawLine(start, start - new Vector2(groundDistanceCheck, 0f));
        }

        start = new Vector2(worldHitBox.xMax, worldHitBox.yMin);
        start.y += yD;
        for (int i = 0; i < linecastCount.y; ++i, start.y += yD)
        {
            if (start.y < yMin)
                Gizmos.color = new Color(0.75f, 0.66f, 0f);
            else
                Gizmos.color = new Color(0.8f, 0.2f, 0.25f);

            Gizmos.DrawLine(start, start + new Vector2(groundDistanceCheck, 0f));
        }
    }
#endif
    #endregion
}

