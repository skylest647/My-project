using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public GameManager gameManager;
        void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Train"))
        {
            Debug.Log("Game Over triggered!");
            gameManager.GameOver();
        }
    }

}
