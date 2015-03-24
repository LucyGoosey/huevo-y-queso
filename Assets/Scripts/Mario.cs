using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
 
[assembly: InternalsVisibleTo("TestMario2")]
 
[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Animator))]
public class Mario : MonoBehaviour {
 
    private Animator animator;
 
    public int playerNum = 1;
 
    public float gravityScale = 1f;

    // Start Variable Block
 
    public Vector2 maxSpeed = new Vector2(7.5f, 10f);
    public float accelSpeed = 20f;
    public float airAccelSpeed = 10f;
    public float groundReverseForce = 2f;
    public float airReverseForce = 2.5f;
    public float groundDragMagic = 0.05f;
    public float groundDragCof = 1f;
    public float airDragMagic = 0f;
    public float jumpForce = 500f;
    public float longJumpForce = 0.1f;
    public float bubblePopDistance = 4f;
 
    private Transform groundPosMid;
    private Transform groundPosLeft;
    private Transform groundPosRight;
    private bool bOnGround = false;
    public bool IsGrounded() { return bOnGround; }
    private bool bWantsToJump = false;
    private int wantedToJumpForFrames = 0;
    public int maxFramesToForgiveJump = 3;
 
    private Transform wallPos;
    private Transform nearWallPos;
    private bool bOnWall = false;
    public bool IsOnwall() { return bOnWall; }
    private bool bNearWall = false;
    private bool bHanging = false;
    public float maxWallHangTime = 2.5f;
    private float wallHangTime = 0f;
    public float wallGrindSpeed = 5f;
    private int fromWall = 0;
    public float timeWallKickBlock = 0.5f;
    private float wallKickTime = 0;
 
    public float longJumpTime = 0.5f;
    private bool bJumpHeld = false;
    internal protected float jumpHeldTime = 0;
    private bool bJumpOffWall = false;
 
    public int maxExtraJumps = 1;
    private int extraJumps = 0;
    public bool bExtraJumpStopsFall = true;
    public bool bJumpsStopY = true;
 
    public float maxGlideTime = 2f;
    internal protected float glideTime = 0f;
    internal protected bool bIsGliding = false;
    public Vector2 glideVelocityModifier = new Vector2(0.5f, 0.5f);
    public float glideGravityScale = 0.5f;
    public bool bGlideKillsY = true;

    public Vector2 preciseJumpVelocityModifier = Vector2.zero;
    public int maxPreciseJumps = 1;
    private int numPreciseJumps = 0;
    public bool bSlamAfterPrecise = true;
    private bool bPrecised = false;
    private bool bSlamming = false;
    public float slamDoublePressTime = 0.1f;
    private float slamPressTime = 0f;
 
    public float maxSlideTime = 1f;
    public float slideDragCof = 0.75f;
    public float minSlideSpeed = 0.75f;
    private float slideTime = 0f;
    private float slideVel = 0f;
    private bool bIsCrouching = false;
    private bool bIsSliding = false;
    public bool bFloatWhileSliding = false;
    public bool bSlideFloatForTime = true;
    public float minSlideFloatSpeed = 0.5f;
    public float endSlideFloatSpeed = 0.2f;
    public float maxSlideFloatTime = 0.5f;
    public float slideFalloffTime = 0.3f;
    private float slideFloatTime = 0f;
 
    public bool bCanDash = true;
    public int maxDashes = 2;
    private int numDashes = 0;
    public bool bDashBlocksInput = false;
    private bool bIsShuffling = false;
    private int dashDir = 0;
    public float dashWaitTime = 1f;
    public float dashMoveTime = 0.3f;
    public bool bShouldDashPause = false;
    public float dashPauseTime = 0.1f;
    private float dashTime = 0f;
    public Vector2 dashForce = new Vector2(15f, 0f);
   
    private bool bIsDead = false;
    public bool IsDead() { return bIsDead; }
 
    public Vector2 wallKickForce = new Vector2(300f, 250f);
    public float longWallKickForce = 5f;
 
    public float deadAccelSpeed = 40f;
    public float deadMoveSpeed = 15f;

    // End Variable Block
 
    public float standStillSpeed = 0.01f;
 
    internal protected bool bUnderDirectControl = false;
 
    void Start ()
    {
#if UNITY_DEBUG
        TestMario tMario = ((GameObject)Instantiate(Resources.Load<GameObject>("TestMario"))).GetComponent<TestMario>();
        tMario.transform.position = transform.position;
        tMario.transform.parent = transform;
        tMario.LinkMario(this);
#endif
 
        animator = GetComponent<Animator>();
 
        groundPosMid = transform.Find("GroundPos");
        groundPosLeft = transform.Find("GroundPosLeft");
        groundPosRight = transform.Find("GroundPosRight");
 
        wallPos = transform.Find("WallPos");
        nearWallPos = transform.Find("NearWallPos");
    }
 
    #region FixedUpdate
 
    protected void FixedUpdate()
    {
        if (bUnderDirectControl && !bIsDead)
        {
            AddHorizontalDrag(groundDragMagic, (slideDragCof * (slideTime / maxSlideTime)) * groundDragCof);
            return;
        }
 
        if (!bIsDead)
        {
            if (bSlamming)
                return;

            if (bIsShuffling && bDashBlocksInput)
            {
                HandleDash(Input.GetAxis("Dash_" + playerNum));
                return;
            }
 
            HandleJump(Input.GetButton("Jump_" + playerNum));
 
            if (!bOnWall && !bIsCrouching)
                HandleMovementAlive(Input.GetAxis("Horizontal_" + playerNum));
            else if (bOnWall)
                HandleMovementWall(Input.GetAxis("Horizontal_" + playerNum));
            else if (bIsSliding && bOnGround)
                AddHorizontalDrag(groundDragMagic, (slideDragCof * (slideTime / maxSlideTime)) * groundDragCof);
 
            if(bCanDash)
                HandleDash(Input.GetAxis("Dash_" + playerNum));
 
            HandlePreciseJump(Input.GetButton("Precise_" + playerNum));
        }
        else
            HandleMovementDead();
    }
 
    internal protected void HandleJump(bool _isJumping)
    {
        if (_isJumping)
        {
            bWantsToJump = true;
            wantedToJumpForFrames = 0;
        }
        else if (bWantsToJump)
        {
            ++wantedToJumpForFrames;
            if (wantedToJumpForFrames > maxFramesToForgiveJump)
                bWantsToJump = false;
        }

        if (!bJumpHeld)
        {
            if (_isJumping || bWantsToJump
                && ((bOnGround || (!bOnGround && extraJumps < maxExtraJumps && _isJumping))
                    || bOnWall || bNearWall))
            {
                animator.Play("Jump");
 
                if (!bOnWall && !bNearWall)
                {
                    if (bOnGround || (!bOnGround && extraJumps < maxExtraJumps && _isJumping))
                    {
                        Vector3 gPos = transform.position + (Vector3)(Vector2.Scale((transform.position - groundPosMid.position).normalized, rigidbody2D.velocity * (maxFramesToForgiveJump * Time.fixedDeltaTime)));
                        bool bNearGround = Physics2D.Linecast(transform.position, gPos, 1 << LayerMask.NameToLayer("Ground"));
                        gPos = transform.position + (Vector3)(Vector2.Scale((transform.position - groundPosLeft.position).normalized, rigidbody2D.velocity * (maxFramesToForgiveJump * Time.fixedDeltaTime)));
                        bNearGround |= Physics2D.Linecast(transform.position, gPos, 1 << LayerMask.NameToLayer("Ground"));
                        gPos = transform.position + (Vector3)(Vector2.Scale((transform.position - groundPosRight.position).normalized, rigidbody2D.velocity * (maxFramesToForgiveJump * Time.fixedDeltaTime)));
                        bNearGround |= Physics2D.Linecast(transform.position, gPos, 1 << LayerMask.NameToLayer("Ground"));

                        if (bNearGround)
                            return;

                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
                        rigidbody2D.AddForce(new Vector2(0f, jumpForce));

                        if(!bOnGround)
                            ++extraJumps;

                        if (_isJumping)
                        {
                            bJumpHeld = true;
                            jumpHeldTime = Time.time;
                        }
                    }
                }
                else if(_isJumping)
                {
                    float dir = transform.localScale.x;

                    if (bOnWall)
                    {
                        fromWall = (int)Mathf.Sign(dir);
                        dir *= -1f;
                    }else
                        fromWall = -(int)Mathf.Sign(dir);

                    rigidbody2D.velocity = Vector2.zero;
                    rigidbody2D.AddForce(new Vector2(wallKickForce.x * dir, wallKickForce.y));
 
                    bOnWall = false;
                    bHanging = false;
                    bJumpOffWall = true;
 
                    rigidbody2D.gravityScale = 1f;
 
                    extraJumps = maxExtraJumps;
                }

                bWantsToJump = false;

                if (transform.parent != null)
                    transform.parent = null;
            }
        }
        else
            if (_isJumping && Time.time - jumpHeldTime < longJumpTime && extraJumps == 0)
                rigidbody2D.AddForce(new Vector2(0f, !bJumpOffWall ? longJumpForce : longWallKickForce));
 
        if (!_isJumping)
        {
            bJumpHeld = false;
            bJumpOffWall = false;
        }
    }
 
    internal protected void HandleDash(float _axis)
    {
        dashTime += Time.fixedDeltaTime;
 
        if (dashTime > dashMoveTime && bIsShuffling)
            bIsShuffling = false;
        if (dashTime > dashWaitTime && dashDir != 0)
            dashDir = 0;
 
        if ((_axis == 0 && !bIsShuffling && dashDir == 0)
            || numDashes > maxDashes)
            return;

        _axis = _axis < 0 ? -1f : 1f;
 
        if (dashTime > dashWaitTime)
        {
            dashTime = 0f;
            bIsShuffling = true;
            dashDir = (int)_axis;

            if(!bOnGround)
                ++numDashes;
        }
 
        if(dashTime < dashPauseTime)
            rigidbody2D.velocity = new Vector2(0f, Physics2D.gravity.y * Time.fixedDeltaTime * -1f);
        else if (dashTime < dashMoveTime)
            rigidbody2D.velocity = new Vector2(dashForce.x * dashDir, Physics2D.gravity.y * Time.fixedDeltaTime * -1f);
    }
 
    internal protected void HandleMovementAlive(float _h)
    {
        if (_h != 0 && Mathf.Sign(_h) != fromWall)
        {            
            if (bOnGround)
                animator.Play("Run");
 
            _h = _h < 0 ? -1 : 1;
            
            if(Mathf.Sign(_h) != Mathf.Sign(transform.localScale.x))
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);

            float aSpeed = (bOnGround ? accelSpeed * groundDragCof : airAccelSpeed);
            if (Mathf.Sign(_h) != Mathf.Sign(rigidbody2D.velocity.x))
                aSpeed *= bOnGround ? groundReverseForce : airReverseForce;

            if (_h * rigidbody2D.velocity.x < maxSpeed.x)
                rigidbody2D.AddForce(Vector2.right * aSpeed * _h);
        }
        else if (bOnGround)
            AddHorizontalDrag(groundDragMagic, groundDragCof);
        else
            AddHorizontalDrag(airDragMagic);
 
        LimitSpeed();
    }
 
    internal protected void HandleMovementWall(float _h)
    {
        if(_h != 0)
            _h = _h < 0 ? -1 : 1;
 
        if (_h != 0 && _h == Mathf.Sign(transform.localScale.x) && wallHangTime < maxWallHangTime)
        {
            bHanging = true;
            animator.Play("WallHang");
 
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, Physics2D.gravity.y * Time.fixedDeltaTime * -1f);
        }
        else
        {
            bHanging = false;
            animator.Play("WallSlide");
 
            if (rigidbody2D.velocity.y > 0)
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
 
            HandleMovementAlive(_h);
        }
 
        if (rigidbody2D.velocity.y < -wallGrindSpeed)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -wallGrindSpeed);
    }
 
    internal protected void HandlePreciseJump(bool _shouldPrecise)
    {
        if (_shouldPrecise)
        {
            if (!bPrecised)
            {
                bPrecised = true;

                if (numPreciseJumps < maxPreciseJumps)
                {
                    if (Time.time - slamPressTime > slamDoublePressTime)
                    {
                        rigidbody2D.velocity = Vector2.Scale(rigidbody2D.velocity, preciseJumpVelocityModifier);
                        slamPressTime = Time.time;
                        if(!bOnGround)
                            ++numPreciseJumps;
                    }
                }
                else if ((Time.time - slamPressTime < slamDoublePressTime || bSlamAfterPrecise)
                        && !bOnGround)
                {
                    rigidbody2D.velocity = Vector2.up * maxSpeed.y * -1f;
                    bSlamming = true;
                }
            }
        }else
            bPrecised = false;
    }
 
    internal protected void HandleMovementDead()
    {
        float h = Input.GetAxis("Horizontal_" + playerNum);
        float v = Input.GetAxis("Vertical_" + playerNum);
 
        rigidbody2D.AddForce(new Vector2(h * deadAccelSpeed, v * deadAccelSpeed));
 
        if (rigidbody2D.velocity.magnitude > deadMoveSpeed)
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * deadMoveSpeed;
    }
 
    private void AddHorizontalDrag(float _dragMagic, float _dragCof = 1f)
    {
        if (Mathf.Abs(rigidbody2D.velocity.x) > standStillSpeed)
        {
            float vX = rigidbody2D.velocity.x;
            // Magic be here
            vX -= vX * (_dragMagic * Mathf.Pow(_dragCof, 2));
 
            if(Mathf.Sign(vX) != Mathf.Sign(rigidbody2D.velocity.x))
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            else
                rigidbody2D.velocity = new Vector2(vX, rigidbody2D.velocity.y);
        }
        else
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            if(bOnGround)
                animator.Play("Still");
        }
    }
 
    private void LimitSpeed()
    {
        float xSpeed = rigidbody2D.velocity.x;
        float ySpeed = rigidbody2D.velocity.y;
 
        // Vector2 max = maxSpeed;
        // if(bIsShuffling && dashDir != 0)
 
 
        if (Mathf.Abs(xSpeed) > maxSpeed.x)
            xSpeed = maxSpeed.x * Mathf.Sign(xSpeed);
        if (Mathf.Abs(ySpeed) > maxSpeed.y)
            ySpeed = maxSpeed.y * Mathf.Sign(ySpeed);
 
        rigidbody2D.velocity = new Vector2(xSpeed, ySpeed);
    }
 
    #endregion
 
    #region Update
 
    void Update()
    {
        if (fromWall != 0)
            wallKickTime += Time.deltaTime;
        if (wallKickTime > timeWallKickBlock)
        {
            fromWall = 0;
            wallKickTime = 0;
        }

        if (bIsDead)
        {
            if (Input.GetButtonDown("Bubble_" + playerNum))
                ToggleBubble();
            return;
        }
 
        CheckGround();
 
        if (!bOnGround)
        {
            CheckWall();
 
            if(!bOnWall && !bUnderDirectControl)
                HandleGlide(Input.GetButton("Glide_" + playerNum));
        }
       
        if((bOnGround || bIsSliding) && !bUnderDirectControl)
            HandleCrouch(Input.GetAxis("Vertical_" + playerNum) < 0);
 
        if (Input.GetButtonDown("Bubble_" + playerNum))
            ToggleBubble();
    }
 
    private void CheckGround()
    {
        bool wasOnGround = bOnGround;
        Collider2D midGround, leftGround, rightGround;

        midGround = Physics2D.Linecast(transform.position, groundPosMid.position, 1 << LayerMask.NameToLayer("Ground")).collider;
        leftGround = Physics2D.Linecast(transform.position, groundPosLeft.position, 1 << LayerMask.NameToLayer("Ground")).collider;
        rightGround = Physics2D.Linecast(transform.position, groundPosRight.position, 1 << LayerMask.NameToLayer("Ground")).collider;

        bOnGround = midGround != null || leftGround != null || rightGround != null;
 
        if (bOnGround && !wasOnGround)
        {
            if (midGround != null)
                transform.parent = midGround.transform;
            else if (leftGround != null)
                transform.parent = leftGround.transform;
            else
                transform.parent = rightGround.transform;

            extraJumps = 0;
 
            wallHangTime = 0f;
            bHanging = false;

            numDashes = 0;
 
            bOnWall = false;
            glideTime = 0f;
            bIsGliding = false;
            fromWall = 0;
            wallKickTime = 0;
 
            numPreciseJumps = 0;
            slamPressTime = 0f;
            bSlamming = false;

            slideFloatTime = 0f;
 
            rigidbody2D.gravityScale = 1f;
 
            animator.Play("Still");
        }
        else if (!bOnGround && wasOnGround && !bOnWall)
            animator.Play("Jump");
    }
 
    private void CheckWall()
    {
        bool wasOnWall = bOnWall;
        bOnWall = Physics2D.Linecast(transform.position, wallPos.position, 1 << LayerMask.NameToLayer("Ground"));
        bNearWall = bOnWall || Physics2D.Linecast(transform.position, nearWallPos.position, 1 << LayerMask.NameToLayer("Ground"));
 
        if (bOnWall && bHanging)
            wallHangTime += Time.fixedDeltaTime;
 
        if (wasOnWall && !bOnWall)
            animator.Play("Jump");
    }
 
    internal protected void HandleGlide(bool _shouldGlide)
    {
        if (bIsGliding)
            glideTime += Time.deltaTime;
 
        if (_shouldGlide && !bIsGliding
            && glideTime < maxGlideTime)
        {
            rigidbody2D.gravityScale = glideGravityScale;
            bIsGliding = true;
           
            rigidbody2D.velocity = Vector2.Scale(rigidbody2D.velocity, glideVelocityModifier);
 
            if(bGlideKillsY)
                if (Mathf.Sign(rigidbody2D.velocity.y) != Mathf.Sign(Physics2D.gravity.y))
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
        }
 
        if(bIsGliding && (!_shouldGlide || glideTime > maxGlideTime))
        {
            rigidbody2D.gravityScale = gravityScale;
            bIsGliding = false;
        }
    }
 
    internal protected void HandleCrouch(bool _shouldCrouch)
    {
        if (!bIsCrouching && _shouldCrouch)
        {
            bIsCrouching = true;
            animator.Play("Crouch");
 
            if ((!bIsSliding && slideTime < maxSlideTime)
                && Mathf.Abs(rigidbody2D.velocity.x) >= maxSpeed.x * minSlideSpeed)
            {
                bIsSliding = true;
                slideVel = rigidbody2D.velocity.x;
            }
 
            if (!bIsSliding)
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        else if (bIsCrouching && !_shouldCrouch)
        {
            bIsCrouching = false;
            bIsSliding = false;
            slideTime = 0f;
 
            if (bFloatWhileSliding)
                rigidbody2D.gravityScale = 1f;
 
            animator.Play("Still");
        }
 
        if (bIsCrouching)
            slideTime += Time.deltaTime;
 
        if (slideTime >= maxSlideTime)
        {
            bIsSliding = false;
            if (bFloatWhileSliding)
                rigidbody2D.gravityScale = 1f;
        }
 
        if (bIsSliding && !bOnGround)
        {
            if (!bSlideFloatForTime)
            {
                float mSFS = maxSpeed.x * minSlideFloatSpeed;
                if (Mathf.Abs(rigidbody2D.velocity.x) < mSFS)
                    rigidbody2D.gravityScale = 1 - ((Mathf.Abs(rigidbody2D.velocity.x) + (endSlideFloatSpeed * maxSpeed.x)) / mSFS);
                else
                    rigidbody2D.gravityScale = 0;
            }
            else
            {
                slideFloatTime += Time.deltaTime;
 
                if (slideFloatTime >= maxSlideFloatTime)
                {
                    bIsSliding = false;
                    rigidbody2D.gravityScale = 1f;
                    slideFloatTime = 0f;
                }
                else if (slideFloatTime > slideFalloffTime)
                    rigidbody2D.gravityScale = (slideFloatTime - slideFalloffTime) / (maxSlideFloatTime - slideFalloffTime);
                else
                    rigidbody2D.gravityScale = 0;
            }
        }
 
    }
 
    private void ToggleBubble()
    {
        if (bIsDead)
        {
#if UNITY_EDITOR
            PopBubble();
#else
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].Equals(gameObject))
                    continue;
                if (players[i].GetComponent<Mario>() != null && players[i].GetComponent<Mario>().IsDead())
                    continue;
 
                if (Vector2.Distance(transform.position, players[i].transform.position) < bubblePopDistance)
                    PopBubble();
            }
#endif
        }
        else
            OnKill();
    }
 
    #endregion
 
    public void OnKill()
    {
        bIsDead = true;
        rigidbody2D.gravityScale = 0;
        rigidbody2D.velocity /= 10;
        collider2D.enabled = false;
        Collider2D[] colls = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colls.Length; ++i)
            colls[i].enabled = false;
 
        DeathBubble db = ((GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Bubble"), transform.position, Quaternion.identity)).GetComponent<DeathBubble>();
        db.transform.parent = transform;
        db.SetPlayer(playerNum);
    }
 
    private void PopBubble()
    {
        bIsDead = false;
        rigidbody2D.gravityScale = 1f;
 
        // TODO Attach to other player if in air
        collider2D.enabled = true;
        Collider2D[] colls = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colls.Length; ++i)
            colls[i].enabled = true;
 
        DeathBubble db = gameObject.GetComponentInChildren<DeathBubble>();
        Destroy(db.gameObject);
    }
}