using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (PlayerMovement.gameActive)
        {
            rb.MovePosition(rb.position + Vector2.down * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
