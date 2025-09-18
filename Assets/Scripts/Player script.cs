using UnityEngine;

using UnityEngine;

public class SubwayPlayer2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f; // Upward movement speed
    public float laneDistance = 2f; // Distance between lanes
    public float sideSpeed = 10f;   // How fast player moves to lane

    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right

    void Update()
    {
        // --- Lane switching ---
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            desiredLane = Mathf.Max(0, desiredLane - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            desiredLane = Mathf.Min(2, desiredLane + 1);

        // --- Target X position based on lane ---
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 2) targetX = laneDistance;

        // --- Smooth movement to lane ---
        Vector3 position = transform.position;
        position.x = Mathf.Lerp(position.x, targetX, sideSpeed * Time.deltaTime);

        // --- Auto-forward movement (up) ---
        position.y += forwardSpeed * Time.deltaTime;

        // --- Apply movement ---
        transform.position = position;
    }
}

