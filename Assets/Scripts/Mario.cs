using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Animator))]
public class Mario : MonoBehaviour {

    private Animator animator;

    public int playerNum = 1;

    public float gravityScale = 1f;

    [SerializeField]
    protected Vector2 maxSpeed = new Vector2(7.5f, 10f);
    [SerializeField]
    protected float accelSpeed = 20f;
    [SerializeField]
    protected float airAccelSpeed = 10f;
    [SerializeField]
    protected float groundDragMagic = 0.05f;
    public float groundDragCof = 1f;
    [SerializeField]
    protected float airDragMagic = 0f;
    [SerializeField]
    protected float jumpForce = 500f;
    [SerializeField]
    protected float longJumpForce = 0.1f;
    [SerializeField]
    protected float bubblePopDistance = 4f;

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
    protected float longJumpTime = 0.5f;
    private bool bJumpHeld = false;
    protected float jumpHeldTime = 0;
    private bool bJumpOffWall = false;

    [SerializeField]
    private int maxJumps = 2;
    private int jumps = 0;

    [SerializeField]
    private float maxGlideTime = 2f;
    private float glideTime = 0f;
    private bool bIsGliding = false;

    [SerializeField]
    private float maxSlideTime = 1f;
    [SerializeField]
    protected float slideDragCof = 0.75f;
    [SerializeField]
    private float minSlideSpeed = 0.75f;
    private float slideTime = 0f;
    private float slideVel = 0f;
    private bool bIsCrouching = false;
    private bool bIsSliding = false;
    public bool bFloatWhileSliding = false;
    
    private bool bIsDead = false;
    public bool IsDead() { return bIsDead; }

    [SerializeField]
    private Vector2 wallKickForce = new Vector2(300f, 250f);
    [SerializeField]
    private float longWallKickForce = 5f;

    [SerializeField]
    protected float deadAccelSpeed = 40f;
    [SerializeField]
    protected float deadMoveSpeed = 15f;

    public float standStillSpeed = 0.01f;

	protected void Start ()
    {
        animator = GetComponent<Animator>();

        groundPosMid = transform.Find("GroundPos");
        groundPosLeft = transform.Find("GroundPosLeft");
        groundPosRight = transform.Find("GroundPosRight");

        wallPos = transform.Find("WallPos");
        nearWallPos = transform.Find("NearWallPos");
	}

    #region Update

    void Update()
    {
        CheckGround();

        if (!bOnGround)
        {
            CheckWall();

            HandleGlide(Input.GetButton("Glide_" + playerNum));
        }else if(bOnGround)
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
    }

    private void HandleGlide(bool _shouldGlide)
    {
        if (bIsGliding)
            glideTime += Time.deltaTime;

        if (_shouldGlide && !bIsGliding
            && glideTime < maxGlideTime)
        {
            rigidbody2D.gravityScale = gravityScale * 0.5f;
            bIsGliding = true;
        }

        if(bIsGliding && (!_shouldGlide || glideTime > maxGlideTime))
        {
            rigidbody2D.gravityScale = gravityScale;
            bIsGliding = false;
        }
    }

    protected void HandleCrouch(bool _shouldCrouch)
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
                if(bFloatWhileSliding)
                    rigidbody2D.gravityScale = 0f;
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
    }

    private void ToggleBubble()
    {
        if (bIsDead)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].Equals(gameObject) || players[i].GetComponent<Mario>().IsDead())
                    continue;

                if (Vector2.Distance(transform.position, players[i].transform.position) < bubblePopDistance)
                    PopBubble(players[i]);
            }
        }
        else
            OnKill();
    }

    #endregion

    #region FixedUpdate

    protected void FixedUpdate()
    {
        if (!bIsDead)
        {
            HandleJump(Input.GetButton("Jump_" + playerNum));

            if (!bOnWall && !bIsCrouching)
                HandleMovementAlive(Input.GetAxis("Horizontal_" + playerNum));
            else if (bOnWall)
                HandleMovementWall(Input.GetAxis("Horizontal_" + playerNum));
            else if (bIsSliding && bOnGround)
                AddHorizontalDrag(groundDragMagic, (slideDragCof * (slideTime / maxSlideTime)) * groundDragCof);
        }
        else
            HandleMovementDead();
    }

    protected void HandleJump(bool _isJumping)
    {
        if (!bJumpHeld)
        {
            if (_isJumping
                && ((bOnGround || (!bOnGround && jumps < maxJumps))
                    || bOnWall || bNearWall))
            {
                animator.Play("Jump");

                if (jumps > 0)
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

                    rigidbody2D.AddForce(new Vector2(wallKickForce.x * dir, wallKickForce.y));

                    bOnWall = false;
                    bHanging = false;
                    bJumpOffWall = true;

                    rigidbody2D.gravityScale = gravityScale;

                    jumps = maxJumps;
                }

                bJumpHeld = true;
                jumpHeldTime = Time.time;

                ++jumps;
            }
        }
        else
            if (_isJumping && Time.time - jumpHeldTime < longJumpTime && jumps == 1)
                rigidbody2D.AddForce(new Vector2(0f, bJumpOffWall ? longJumpForce : longWallKickForce));

        if (!_isJumping)
        {
            bJumpHeld = false;
            bJumpOffWall = false;
        }
    }

    protected void HandleMovementAlive(float _h)
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

    protected void HandleMovementWall(float _h)
    {
        if(_h != 0)
            _h = _h < 0 ? -1 : 1;

        if (_h == Mathf.Sign(transform.localScale.x) && wallHangTime < maxWallHangTime)
        {
            bHanging = true;
            animator.Play("WallHang");

            rigidbody2D.gravityScale = 0f;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
        }
        else
        {
            bHanging = false;
            animator.Play("WallSlide");

            rigidbody2D.gravityScale = gravityScale;
            HandleMovementAlive(_h);
        }

        if (rigidbody2D.velocity.y < -wallGrindSpeed)
            rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -wallGrindSpeed);
    }

    protected void HandleMovementDead()
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

        if (Mathf.Abs(xSpeed) > maxSpeed.x)
            xSpeed = maxSpeed.x * Mathf.Sign(xSpeed);
        if (Mathf.Abs(ySpeed) > maxSpeed.y)
            ySpeed = maxSpeed.y * Mathf.Sign(ySpeed);

        rigidbody2D.velocity = new Vector2(xSpeed, ySpeed);
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

    private void PopBubble(GameObject _player)
    {
        bIsDead = false;
        rigidbody2D.gravityScale = gravityScale;

        // TODO Attach to other player if in air
        collider2D.enabled = true;
        Collider2D[] colls = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colls.Length; ++i)
            colls[i].enabled = true;

        DeathBubble db = gameObject.GetComponentInChildren<DeathBubble>();
        Destroy(db.gameObject);
    }
}
