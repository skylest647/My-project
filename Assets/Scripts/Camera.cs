using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.1f;
    private Vector3 offset;

    void Start()
    {
        // Maintain initial offset
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
    }
}
