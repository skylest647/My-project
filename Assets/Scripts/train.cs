using UnityEngine;

/// <summary>
/// Simple Train behaviour: can be stationary for a duration (sitting) or move along X axis.
/// Attach to a train prefab. The spawner will set properties at spawn time.
/// </summary>
public class Train : MonoBehaviour
{
    public enum TrainState { Idle, Moving }

    [Header("Movement")]
    public float speed = 5f;
    public Vector3 moveDirection = Vector3.right;

    [Header("Sitting / Idle")]
    public bool isSitting = false;
    public float sitDuration = 3f;

    private float sitTimer = 0f;
    private TrainState state = TrainState.Idle;

    void OnEnable()
    {
        sitTimer = 0f;
        state = isSitting ? TrainState.Idle : TrainState.Moving;
    }

    void Update()
    {
        if (!PlayerMovement.gameActive) return;
        if (isSitting)
        {
            sitTimer += Time.deltaTime;
            if (sitTimer >= sitDuration)
            {
                isSitting = false;
                state = TrainState.Moving;
            }
            return;
        }

        if (state == TrainState.Moving)
        {
            transform.position += moveDirection.normalized * speed * Time.deltaTime;
        }
    }

    // Reset helper called by spawner or pooler
    public void Initialize(bool sitting, float duration, float moveSpeed, Vector3 direction)
    {
        isSitting = sitting;
        sitDuration = duration;
        speed = moveSpeed;
        moveDirection = direction;
        sitTimer = 0f;
        state = isSitting ? TrainState.Idle : TrainState.Moving;
    }
}
