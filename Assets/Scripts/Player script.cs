using UnityEngine;

public class SubwayPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f;
    public float laneDistance = 4f; 
    public float sideSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private int desiredLane = 1;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
        moveDirection.z = forwardSpeed;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            desiredLane = Mathf.Max(0, desiredLane - 1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            desiredLane = Mathf.Min(2, desiredLane + 1);
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).x * sideSpeed;
        moveVector.z = forwardSpeed;

        
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveVector.y = verticalVelocity;

        // Apply movement
        controller.Move(moveVector * Time.deltaTime);
    }
}