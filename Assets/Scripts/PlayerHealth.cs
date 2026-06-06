using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance; // Singleton instance for global access
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Damage Cooldown (I-Frames)")]
    public float damageCooldown = 1f; // Cannot take damage more than once per second
    private float cooldownTimer;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Toto spawned with {currentHealth}/{maxHealth} HP!");
    }

    void Update()
    {
        // Handle invincibility cooldown clock
        if (isInvincible)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public static void BoostHealth(int boost)
    {
        // 1. If the static instance shortcut isn't set yet, hunt for it in the scene right now
        if (instance == null)
        {
            instance = Object.FindFirstObjectByType<PlayerHealth>();
        }

        // 2. Safely apply the health modifications
        if (instance != null)
        {
            instance.maxHealth += boost;
            instance.currentHealth += boost; // Boosts both max and current together!
            
            Debug.Log($"[Health Boost] Toto's health increased by {boost}! Current HP: {instance.currentHealth}/{instance.maxHealth}");
        } 
        else
        {
            // 3. Clear error tracking if the script isn't anywhere in the active scene
            Debug.LogError("CRITICAL: Tried to BoostHealth, but no GameObject with a PlayerHealth script exists in the scene!");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        // If Toto recently got hit, ignore new damage for now
        if (isInvincible) return;

        currentHealth -= damageAmount;
        Debug.Log($"Toto took hit! Current HP: {currentHealth}/{maxHealth}");

        // Trigger invincibility cooldown
        isInvincible = true;
        cooldownTimer = damageCooldown;

        // Check for Game Over condition
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.LogError("!!! TOTO DIED !!! Game Over.");

        // Stop Toto's physical movement sliding
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        this.enabled = false; // Disables this script upon death

        // UPDATED FOR UNITY 6: Changed FindObjectOfType to FindFirstObjectByType
        GameOverController uiController = FindFirstObjectByType<GameOverController>();
        if (uiController != null)
        {
            uiController.TriggerGameOver();
        }
    }

    public void ApplyHealthBoost(string statType, int value)
    {
        if (statType.ToLower() == "heal")
        {
            // Heal Toto but make sure he doesn't go over his maximum allowed health limit
            currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
            Debug.Log($"[REWARD] Toto healed by {value}! Current HP: {currentHealth}/{maxHealth}");
        }
        else if (statType.ToLower() == "maxhealth")
        {
            // Permanently increase his heart capacity container and give him a free heal for that amount
            maxHealth += value;
            currentHealth += value;
            Debug.Log($"[REWARD] Max HP increased by {value}! New Max HP: {maxHealth} (Current HP bumped to: {currentHealth})");
        }
    }

    // ====================================================================
    // 🔥 NEW ADDITION: RESET HEALTH FUNCTION FOR WAVE RETRY SYSTEM 🔥
    // ====================================================================
    public void ResetHealthToMax()
    {
        // 1. Re-enable this script component so Toto can track damage frames again!
        this.enabled = true;

        // 2. Reset health metrics back to full container capacity status
        currentHealth = maxHealth;

        // 3. Clear out old invincibility flags so he's clean for the new try
        isInvincible = false;
        cooldownTimer = 0f;

        Debug.Log($"[RETRY] Toto health fully restored! Current HP: {currentHealth}/{maxHealth}");

    }
}