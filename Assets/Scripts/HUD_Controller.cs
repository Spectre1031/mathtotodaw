using UnityEngine;
using TMPro;

public class GameHUDController : MonoBehaviour
{
    [Header("UI Text References")]
    public TMP_Text healthText;
    public TMP_Text maxHealthText;
    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text movementText;
    public TMP_Text orbTrackerText;
    public TMP_Text maxOrbTrackerText;
    public TMP_Text timerText;

    [Header("Player Script References")]
    public PlayerHealth playerHealth;
    public PlayerMovement2D playerMovement;

    private LevelManager levelManager;
    private TimeTracker timeTracker;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            playerMovement = playerObj.GetComponent<PlayerMovement2D>();
        }
        else
        {
            Debug.LogError("HUD Controller could not find a GameObject tagged 'Player'!");
        }

        levelManager = Object.FindFirstObjectByType<LevelManager>();
        timeTracker = Object.FindFirstObjectByType<TimeTracker>();
    }

    void Update()
    {
        // 1. Live Update Core Health Stats
        if (playerHealth != null && healthText != null)
        {
            healthText.text = $"{playerHealth.currentHealth}";
            maxHealthText.text = $"/{playerHealth.maxHealth}";
        }

        // 2. Live Update Player Combat & Attribute Stats from PlayerMovement2D
        if (playerMovement != null)
        {
            if (attackText != null)
            {
                attackText.text = $"ATK: {playerMovement.attackPower}";
            }

            if (defenseText != null)
            {
                defenseText.text = $"DEF: {playerMovement.defense}";
            }

            if (movementText != null)
            {
                movementText.text = $"MOV: {playerMovement.movementSpeed:F1}";
            }
        }

        // 3. Update Wave Orb Metric Progression
        if (levelManager != null && orbTrackerText != null)
        {
            orbTrackerText.text = $"{levelManager.GetSolvedCount()}";
            maxOrbTrackerText.text = $"/{levelManager.totalOrbsInLevel}";
        }

        // 4. Format and Display Wave Time Clock Remaining
        if (timeTracker != null && timerText != null)
        {
            float timeToDisplay = timeTracker.timeRemaining;
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}