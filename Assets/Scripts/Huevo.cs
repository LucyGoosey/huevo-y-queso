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
        CalculateVelocity();

        // Begin physics check

        // TODO Prioritise direction check order depending on velocity
        GroundCheck(0, -1);
        GroundCheck(0, 1);
        GroundCheck(-1, 0);
        GroundCheck(1, 0);

        // Limit the velocity to the max speed
        if (Mathf.Abs(velocity.x) > maxSpeed.x)
            velocity.x = maxSpeed.x * Mathf.Sign(velocity.x);
        if (Mathf.Abs(velocity.y) > maxSpeed.y)
            velocity.y = maxSpeed.y * Mathf.Sign(velocity.y);

        transform.position += (Vector3)(velocity * Time.deltaTime);
        worldHitBox.center = transform.position + new Vector3(0f, (hitboxWidthHeight.y / 2f));
    }

    private void CalculateVelocity()
    {
        velocity += gravity * Time.deltaTime;

        // Check for horizontal input, and apply acceleration if necessary
        if (inHandler.Horizontal != 0f)
            velocity.x += accel * inHandler.Horizontal * Time.deltaTime;
    }

    private void GroundCheck(int _xDir, int _yDir)
    {
        if (_xDir != 0 && _yDir != 0 || _xDir == 0 && _yDir == 0)
        {
            #if UNITY_DEBUG
            Debug.LogWarning("Invalid parameters passed to Huevo.GroundCheck().\nOnly x XOR y can have a value != 0.");
            #endif
            return;
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
        for (int i = 0; i < (_xDir != 0 ? linecastCount.x : linecastCount.y); ++i)
        {
            Vector2 end = Vector2.zero;
            if (_xDir != 0)
                end = start + new Vector2(xD * _xDir, 0f);
            else if(_yDir != 0)
                end = start + new Vector2(0f, yD * _yDir);

            groundHit = Physics2D.Linecast(start, end, 1 << LayerMask.NameToLayer("Ground"));

            #if UNITY_DEBUG
            if (groundHit)
            {
                Debug.DrawLine(start, end, Color.red);
                break;
            }else
                Debug.DrawLine(start, end);
            #else
            if (groundHit)
                break;
            #endif

            if (_xDir == 0)
                start.x += xD;
            else if (_yDir == 0)
                start.y += yD;
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

