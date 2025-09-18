using UnityEngine;
using UnityEngine.InputSystem;

public class SubwayPlayer2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float laneDistance = 2f; // Distance between lanes
    public float sideSpeed = 10f;   // How fast player moves to lane

    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right
    private Vector2 moveInput;

    // Called automatically by Input System when Move action is performed
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Horizontal input for lane switching
        if (moveInput.x < 0)
            desiredLane = Mathf.Max(0, desiredLane - 1);
        else if (moveInput.x > 0)
            desiredLane = Mathf.Min(2, desiredLane + 1);
    }

    void Update()
    {
        // Target X position based on lane
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 2) targetX = laneDistance;

        // Smooth horizontal movement
        Vector3 position = transform.position;
        position.x = Mathf.Lerp(position.x, targetX, sideSpeed * Time.deltaTime);

        // Apply position
        transform.position = position;
    }
}
