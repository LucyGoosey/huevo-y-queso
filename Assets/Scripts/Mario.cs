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
    protected float groundDrag = 5f;
    [SerializeField]
    protected float airDrag = 0f;
    [SerializeField]
    protected float jumpForce = 500f;
    [SerializeField]
    protected float longJumpForce = 0.1f;
    [SerializeField]
    protected float bubblePopDistance = 4f;

    private Transform groundPos;
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
    private float slideDrag = 2.5f;
    private float slideTime = 0f;
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

	protected void Start ()
    {
        animator = GetComponent<Animator>();

        groundPos = transform.Find("GroundPos");
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
        bOnGround = Physics2D.Linecast(transform.position, groundPos.position, 1 << LayerMask.NameToLayer("Ground"));

        if (bOnGround)
        {
            jumps = 0;

            wallHangTime = 0f;
            bHanging = false;

            bOnWall = false;
            glideTime = 0f;
            bIsGliding = false;

            rigidbody2D.gravityScale = gravityScale;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                animator.Play("Still");
        }
    }

    private void CheckWall()
    {
        bool wasOnWall = bOnWall;
        bOnWall = Physics2D.Linecast(transform.position, wallPos.position, 1 << LayerMask.NameToLayer("Ground"));
        bNearWall = bOnWall || Physics2D.Linecast(transform.position, nearWallPos.position, 1 << LayerMask.NameToLayer("Ground"));

        if (bOnWall && bHanging)
            wallHangTime += Time.fixedDeltaTime;
        if (!bOnWall && wasOnWall && !bOnGround)
            animator.Play("Jump");
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
        if (_shouldCrouch)
            animator.Play("Crouch");
        if (bIsCrouching && !_shouldCrouch)
        {
            animator.Play("Still");
            bIsCrouching = false;
        }

        if (bIsCrouching)
            slideTime += Time.deltaTime;

        if (!bIsCrouching && _shouldCrouch)
        {
            bIsCrouching = true;
            if (!bIsSliding && slideTime < maxSlideTime)
            {
                bIsSliding = true;
                if(bFloatWhileSliding)
                    rigidbody2D.gravityScale = 0f;
            }
        }

        if ((bIsCrouching && !_shouldCrouch)
            || slideTime >= maxSlideTime)
        {
            bIsSliding = false;
            slideTime = 0f;
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
        if (bIsSliding)
            AddHorizontalDrag(slideDrag);

        if (_h != 0)
        {
            if (bOnGround)
                animator.Play("Run");

            _h = _h < 0 ? -1 : 1;

            transform.localScale = new Vector3(_h, transform.localScale.y, transform.localScale.z);

            if (_h * rigidbody2D.velocity.x < maxSpeed.x)
                rigidbody2D.AddForce(Vector2.right * (bOnGround ? accelSpeed : accelSpeed / 2) * _h);
        }else if (bOnGround && !bIsSliding)
            AddHorizontalDrag(groundDrag);
        if (!bOnGround)
            AddHorizontalDrag(airDrag);

        LimitSpeed();
    }

    private void AddHorizontalDrag(float _drag)
    {
        if (rigidbody2D.velocity.x != 0)
        {
            float oldDir = Mathf.Sign(rigidbody2D.velocity.x);
            rigidbody2D.AddForce(Vector2.right * -oldDir * _drag);

            if ((oldDir > 0 && rigidbody2D.velocity.x < 0)
                || (oldDir < 0 && rigidbody2D.velocity.x > 0))
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
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
