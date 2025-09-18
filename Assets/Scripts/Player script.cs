using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float laneDistance = 2f;
    private int desiredLane = 1;
    public float moveSpeed = 10f;
    private float inputCooldown = 0.25f;
    private float lastInputTime = 0f;

    void Update()
    {
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

        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 2) targetX = laneDistance;

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
