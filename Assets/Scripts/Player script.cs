using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float laneDistance = 2f;
    private int desiredLane = 1;
    public float moveSpeed = 10f;
    private float inputCooldown = 0.25f;
    private float lastInputTime = 0f;
    public static bool gameActive = true;

    public Rigidbody2D rb; 

    void Update()
    {
        if (!gameActive) return;

        float h = Input.GetAxis("Horizontal");

        if (Time.time - lastInputTime > inputCooldown)
        {
            if (h > 0.5f)
            {
                desiredLane = Mathf.Min(2, desiredLane + 1);
                lastInputTime = Time.time;
            }
            else if (h < -0.5f)
            {
                desiredLane = Mathf.Max(0, desiredLane - 1);
                lastInputTime = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        if (!gameActive) return;

        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 2) targetX = laneDistance;

        Vector2 targetPos = new Vector2(targetX, rb.position.y);
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime));
    }
}
