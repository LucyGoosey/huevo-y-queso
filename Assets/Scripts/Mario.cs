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

    [SerializeField]
    private float vineSwingForce = 1f;
    private Transform handPos;
    private VinePiece holdingVine = null;
    private bool bOnVine = false;
    
    private bool bIsDead = false;
    public bool IsDead() { return bIsDead; }

    private float halfJumpForce;
    private float halfLongJumpForce;

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
        handPos = transform.Find("HandPos");
        BoxCollider2D box = handPos.gameObject.GetComponent<BoxCollider2D>();

        halfJumpForce = jumpForce / 2;
        halfLongJumpForce = longJumpForce / 2;
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
                bIsSliding = true;
        }

        if ((bIsCrouching && !_shouldCrouch)
            || slideTime >= maxSlideTime)
        {
            bIsSliding = false;
            slideTime = 0f;
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
            float h = Input.GetAxis("Horizontal_" + playerNum);
            float v = Input.GetAxis("Vertical_" + playerNum);

            if (bOnVine)
                HandleMovementVine(h, v);
            else if (bOnWall)
                HandleMovementWall(h);
            else if (!bIsCrouching)
                HandleMovementAlive(h);
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
                    || bOnWall || bNearWall || bOnVine))
            {
                animator.Play("Jump");

                if (jumps > 0)
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

                if (bOnVine)
                {
                    // Jump off vine
                }
                else if (bOnWall || bNearWall)
                {
                    float dir = transform.localScale.x;

                    if (bOnWall)
                        dir *= -1f;

                    rigidbody2D.AddForce(new Vector2(halfJumpForce * dir, halfJumpForce));

                    bOnWall = false;
                    bHanging = false;
                    bJumpOffWall = true;

                    rigidbody2D.gravityScale = gravityScale;

                    jumps = maxJumps;
                }else
                {
                    rigidbody2D.AddForce(new Vector2(0f, jumpForce));
                    bOnGround = false;
                }
                

                bJumpHeld = true;
                jumpHeldTime = Time.time;

                ++jumps;
            }
        }
        else
            if (_isJumping && Time.time - jumpHeldTime < longJumpTime && jumps == 1)
                rigidbody2D.AddForce(new Vector2(0f, bJumpOffWall ? longJumpForce : halfLongJumpForce));

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

    protected void HandleMovementVine(float _h, float _v)
    {
        if (holdingVine == null)
        {
            bOnVine = false;
            HandleMovementAlive(_h);
            return;
        }

        if (_h != 0)
        {
            _h = _h > 0 ? 1 : -1;

            Vector2 dir = (holdingVine.GetAnchor().transform.position - holdingVine.transform.position).normalized;

            Quaternion q = Quaternion.Euler(0, 0, 90 * -_h);
            dir = q * dir;

            if (dir.y > 0)
                dir.y *= -1f;

            holdingVine.rigidbody2D.AddForce(vineSwingForce * dir);
            Debug.DrawLine(transform.position, transform.position + (Vector3)(dir * 500f), Color.magenta);
        }
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

    public void VineHit(VinePiece _vine)
    {
        if (bOnVine || holdingVine != null)
            return;

        bOnVine = true;
        holdingVine = _vine;
        holdingVine.rigidbody2D.mass += rigidbody2D.mass;

        handPos.collider2D.enabled = false;
        rigidbody2D.isKinematic = true;
        transform.position = _vine.transform.position - handPos.localPosition;
        transform.parent = _vine.transform;

        holdingVine.joint.connectedBody = holdingVine.GetAnchor().rigidbody2D;
        holdingVine.joint.distance = Vector2.Distance(holdingVine.transform.position, holdingVine.GetAnchor().transform.position);

        holdingVine.connectedTo.EnableReverseJoint();
    }
}
