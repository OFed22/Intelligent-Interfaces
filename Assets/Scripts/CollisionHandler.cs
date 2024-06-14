using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private bool gameIsOver = false;
    [SerializeField] private GameOverUIManager gameOverUIManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && !gameIsOver)
        {
            Debug.Log("Hut!");
            gameIsOver = true;
            Time.timeScale = 0f; // Pause the game
            
            if (gameOverUIManager != null)
            {
                gameOverUIManager.GameOver();
            }
            else
            {
                Debug.LogError("GameOverUIManager instance is null.");
            }
        }
    }
}