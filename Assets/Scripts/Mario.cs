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

    [SerializeField]
    internal protected Vector2 maxSpeed = new Vector2(7.5f, 10f);
    [SerializeField]
    internal protected float accelSpeed = 20f;
    [SerializeField]
    internal protected float airAccelSpeed = 10f;
    [SerializeField]
    internal protected float groundDragMagic = 0.05f;
    public float groundDragCof = 1f;
    [SerializeField]
    internal protected float airDragMagic = 0f;
    [SerializeField]
    internal protected float jumpForce = 500f;
    [SerializeField]
    internal protected float longJumpForce = 0.1f;
    [SerializeField]
    internal protected float bubblePopDistance = 4f;

    private Transform groundPosMid;
    private Transform groundPosLeft;
    private Transform groundPosRight;
    private bool bOnGround = false;
    public bool IsGrounded() { return bOnGround; }

    private Transform wallPos;
    private Transform nearWallPos;
    private bool bOnWall = false;
    public bool IsOnwall() { return bOnWall; }
    private bool bNearWall = false;
    private bool bHanging = false;
    [SerializeField]
    private float maxWallHangTime = 2.5f;
    private float wallHangTime = 0f;
    [SerializeField]
    private float wallGrindSpeed = 5f;

    [SerializeField]
    internal protected float longJumpTime = 0.5f;
    private bool bJumpHeld = false;
    internal protected float jumpHeldTime = 0;
    private bool bJumpOffWall = false;

    [SerializeField]
    private int maxJumps = 2;
    private int jumps = 0;
    public bool bExtraJumpStopsFall = true;
    public bool bJumpsStopY = true;

    [SerializeField]
    internal protected float maxGlideTime = 2f;
    internal protected float glideTime = 0f;
    internal protected bool bIsGliding = false;
    public Vector2 glideVelocityModifier = new Vector2(0.5f, 0.5f);
    public float glideGravityScale = 0.5f;
    public bool bGlideKillsY = true;
    public Vector2 preciseJumpVelocityModifier = Vector2.zero;
    private bool bPrecised = false;
    public float slamDoublePressTime = 0.1f;
    private float slamPressTime = 0f;

    [SerializeField]
    private float maxSlideTime = 1f;
    [SerializeField]
    internal protected float slideDragCof = 0.75f;
    [SerializeField]
    private float minSlideSpeed = 0.75f;
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

    [SerializeField]
    private bool bCanShuffle = true;
    [SerializeField]
    private bool bShuffleBlocksInput = false;
    private bool bIsShuffling = false;
    private int shuffleDir = 0;
    [SerializeField]
    private float shuffleWaitTime = 1f;
    [SerializeField]
    private float shuffleMoveTime = 0.3f;
    [SerializeField]
    private bool bShouldShufflePause = false;
    [SerializeField]
    private float shufflePauseTime = 0.1f;
    private float shuffleTime = 0f;
    [SerializeField]
    private Vector2 shuffleForce = new Vector2(15f, 0f);
    
    private bool bIsDead = false;
    public bool IsDead() { return bIsDead; }

    [SerializeField]
    private Vector2 wallKickForce = new Vector2(300f, 250f);
    [SerializeField]
    private float longWallKickForce = 5f;

    [SerializeField]
    internal protected float deadAccelSpeed = 40f;
    [SerializeField]
    internal protected float deadMoveSpeed = 15f;

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
            if (bIsShuffling && bShuffleBlocksInput)
            {
                HandleShuffle(Input.GetAxis("Shuffle_" + playerNum));
                return;
            }

            HandleJump(Input.GetButton("Jump_" + playerNum));

            if (!bOnWall && !bIsCrouching)
                HandleMovementAlive(Input.GetAxis("Horizontal_" + playerNum));
            else if (bOnWall)
                HandleMovementWall(Input.GetAxis("Horizontal_" + playerNum));
            else if (bIsSliding && bOnGround)
                AddHorizontalDrag(groundDragMagic, (slideDragCof * (slideTime / maxSlideTime)) * groundDragCof);

            if(bCanShuffle)
                HandleShuffle(Input.GetAxis("Shuffle_" + playerNum));

            HandlePreciseJump(Input.GetButton("Precise_" + playerNum));
        }
        else
            HandleMovementDead();
    }

    internal protected void HandleJump(bool _isJumping)
    {
        if (!bJumpHeld)
        {
            if (_isJumping
                && ((bOnGround || (!bOnGround && jumps < maxJumps))
                    || bOnWall || bNearWall))
            {
                animator.Play("Jump");

                if ((jumps > 0 && bExtraJumpStopsFall) || bJumpsStopY)
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

                if (!bOnWall && !bNearWall)
                {
                    rigidbody2D.AddForce(new Vector2(0f, jumpForce));

                    bOnGround = false;
                }
                else
                {
                    float dir = transform.localScale.x;

                    if (bOnWall)
                        dir *= -1f;
                    if(bJumpsStopY)
                        rigidbody2D.velocity = Vector2.zero;

                    rigidbody2D.AddForce(new Vector2(wallKickForce.x * dir, wallKickForce.y));

                    bOnWall = false;
                    bHanging = false;
                    bJumpOffWall = true;

                    rigidbody2D.gravityScale = 1f;

                    jumps = maxJumps;
                }

                bJumpHeld = true;
                jumpHeldTime = Time.time;

                ++jumps;
            }
        }
        else
            if (_isJumping && Time.time - jumpHeldTime < longJumpTime && jumps == 1)
                rigidbody2D.AddForce(new Vector2(0f, !bJumpOffWall ? longJumpForce : longWallKickForce));

        if (!_isJumping)
        {
            bJumpHeld = false;
            bJumpOffWall = false;
        }
    }

    internal protected void HandleShuffle(float _axis)
    {
        shuffleTime += Time.fixedDeltaTime;

        if (shuffleTime > shuffleMoveTime && bIsShuffling)
            bIsShuffling = false;
        if (shuffleTime > shuffleWaitTime && shuffleDir != 0)
            shuffleDir = 0;

        if (_axis == 0 && !bIsShuffling && shuffleDir == 0)
            return;

        if (shuffleTime > shuffleWaitTime)
        {
            shuffleTime = 0f;
            bIsShuffling = true;
            shuffleDir = (int)_axis;
        }

        _axis = _axis < 0 ? -1f : 1f;

        if(shuffleTime < shufflePauseTime)
            rigidbody2D.velocity = new Vector2(0f, Physics2D.gravity.y * Time.fixedDeltaTime * -1f);
        else if (shuffleTime < shuffleMoveTime)
            rigidbody2D.velocity = new Vector2(shuffleForce.x * shuffleDir, Physics2D.gravity.y * Time.fixedDeltaTime * -1f);
    }

    internal protected void HandleMovementAlive(float _h)
    {
        if (_h != 0)
        {
            if (bOnGround)
                animator.Play("Run");

            _h = _h < 0 ? -1 : 1;

            transform.localScale = new Vector3(_h, transform.localScale.y, transform.localScale.z);

            if (_h * rigidbody2D.velocity.x < maxSpeed.x)
                rigidbody2D.AddForce(Vector2.right * (bOnGround ? accelSpeed * groundDragCof : airAccelSpeed) * _h);
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

        if (_h == Mathf.Sign(transform.localScale.x) && wallHangTime < maxWallHangTime)
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

                if (Time.time - slamPressTime > slamDoublePressTime)
                {
                    rigidbody2D.velocity = Vector2.Scale(rigidbody2D.velocity, preciseJumpVelocityModifier);
                    slamPressTime = Time.time;
                }
                else
                {
                    rigidbody2D.velocity = Vector2.up * maxSpeed.y * -1f;
                    slamPressTime = 0f;
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
        // if(bIsShuffling && shuffleDir != 0)


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
        bOnGround = Physics2D.Linecast(transform.position, groundPosMid.position, 1 << LayerMask.NameToLayer("Ground"))
                    || Physics2D.Linecast(transform.position, groundPosLeft.position, 1 << LayerMask.NameToLayer("Ground"))
                    || Physics2D.Linecast(transform.position, groundPosRight.position, 1 << LayerMask.NameToLayer("Ground"));

        if (bOnGround && !wasOnGround)
        {
            jumps = 0;

            wallHangTime = 0f;
            bHanging = false;

            bOnWall = false;
            glideTime = 0f;
            bIsGliding = false;

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
