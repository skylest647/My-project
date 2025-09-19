using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;  // Assign in Inspector

    void Start()
    {
        gameOverCanvas.SetActive(false); // Hide at start
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true); // Show when game is over
    }
}