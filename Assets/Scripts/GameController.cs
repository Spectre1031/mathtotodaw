using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static bool isAnswering = false;
    public static bool isGameOver = false;

    public static bool isPaused = false;

    public GameObject gameOverPanel; // Drag your UI Panel here

    void OnEnable()
    {
        TimeTracker.OnTimeOut += TriggerGameOver;
    }
    void Start()
    {
        // Ensure the screen is hidden when the level first loads!
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            isGameOver = false; 
            isPaused = false;
        }
        Time.timeScale = 1f; // Make sure physics/time flows normally
    }

    public void TriggerGameOver()
    {
        Debug.Log("Game Over triggered! Time ran out.");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Pop up the panel layout
            isGameOver = true;
        }
        // Time.timeScale = 0f; // Freeze all movement and timers perfectly!
    }

    // This method will be linked directly to your UI Button's Click listener
    public void RestartGame()
    {
        Time.timeScale = 1f; // Always unfreeze time BEFORE reloading!

        // Reloads whatever scene you currently have open automatically
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}