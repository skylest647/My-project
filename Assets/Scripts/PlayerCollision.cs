using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Train"))
        {
            Debug.Log("Game Over triggered!");
            gameManager.GameOver();
            PlayerMovement.gameActive = false; // stop player movement
        }
    }
}
