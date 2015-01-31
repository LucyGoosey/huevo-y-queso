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
    private Rect worldHitBox;
    public Vector2 hitboxWidthHeight = new Vector2(1.6f, 1.6f);
    private float groundDistanceCheck = 0.1f;
    public Vector2 linecastCount = new Vector2(16, 16);

    public int playerNum = 0;

    private Vector2 velocity = Vector2.zero;
    public Vector2 gravity = new Vector2(0f, -24f);
    public Vector2 maxSpeed = new Vector2(10f, 10f);
    public float accel = 40f;
    public float jumpForce = 9f;

    private InputHandler inHandler;

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
        // Calculate velocity

        worldHitBox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));
        velocity += gravity * Time.deltaTime;
        if (inHandler.Horizontal != 0f)
            velocity.x += accel * inHandler.Horizontal * Time.deltaTime;

        // Begin physics check

        // TODO Figure out this business
        GroundCheckVertical(false);
        GroundCheckVertical(true);

        GroundCheckHorizontal(false);
        GroundCheckHorizontal(true);

        if (velocity.x > maxSpeed.x)
            velocity.x = maxSpeed.x;
        if (velocity.y > maxSpeed.y)
            velocity.y = maxSpeed.y;

        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void GroundCheckVertical(bool bUp)
    {
        Rect testHitbox = worldHitBox;
        testHitbox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));
        testHitbox.center += velocity * Time.deltaTime;

        Vector2 start = new Vector2(testHitbox.xMin, testHitbox.center.y);
        float xD = testHitbox.width / (linecastCount.x + 1);
        float yD = (testHitbox.height / 2f) + groundDistanceCheck;
        start.x += xD;

        RaycastHit2D groundHit = new RaycastHit2D();
        for (int i = 0; i < linecastCount.x; ++i)    // TODO Spiral out from center
        {
            Vector2 end = bUp ? start + new Vector2(0f, yD) : start - new Vector2(0f, yD);
            Debug.DrawLine(start, end);
            groundHit = Physics2D.Linecast(start, end, 1 << LayerMask.NameToLayer("Ground"));

            if (groundHit.collider)
                break;

            start.x += xD;
        }

        if (groundHit)
            if(bUp ? velocity.y > 0f : velocity.y < 0f)
            {
                velocity.y = 0f;
    
                transform.position = new Vector3(transform.position.x, bUp ? groundHit.collider.bounds.min.y - hitboxWidthHeight.y: groundHit.collider.bounds.max.y);
            }
    }

    private void GroundCheckHorizontal(bool bRight)
    {
        Rect testHitbox = worldHitBox;
        testHitbox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));
        testHitbox.center += velocity * Time.deltaTime;

        Vector2 start = new Vector2(testHitbox.center.x, testHitbox.yMin);
        float xD = (testHitbox.width / 2f) + groundDistanceCheck;
        float yD = testHitbox.height / (linecastCount.y + 1);
        start.y += yD;

        RaycastHit2D groundHit = new RaycastHit2D();
        for (int i = 0; i < linecastCount.y; ++i)    // TODO Spiral out from center
        {
            Vector2 end = bRight ? start + new Vector2(xD, 0f) : start - new Vector2(xD, 0f);
            Debug.DrawLine(start, end);
            groundHit = Physics2D.Linecast(start, end, 1 << LayerMask.NameToLayer("Ground"));

            if (groundHit)
                break;

            start.y += yD;
        }

        if (groundHit)
        {
            if (bRight ? velocity.x > 0f : velocity.x < 0f)
                velocity.x = 0f;

            transform.position = new Vector3(bRight ? groundHit.collider.bounds.min.x - (worldHitBox.width / 2f) 
                                                    : groundHit.collider.bounds.max.x + (worldHitBox.width / 2f),
                                                    transform.position.y);
        }
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

