using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public UnityEvent onLevelEnd;
    [Header("Level Progress Settings")]
    public int totalOrbsInLevel = 10;
    private int orbsSolved = 0;
    public int currentWave = 1;

    [Header("Weapon Selection UI")]
    public GameObject weaponSelectionPanel; // Drag Canvas_WeaponShop here
    public TMP_Text selectionTimerText;    // Drag SelectionTimerText here
    private float buyTimeRemaining = 20f;
    private bool isInBuyTime = false;

    [Header("UI Document Connections")]
    public TMP_Text orbTrackerText;
    public TMP_Text waveBannerText;
    public GameObject exitPortal;
    public GameObject waveCompletePanel; // Drag Canvas_VictoryScreen here
    public GameObject stats;
    public GameObject gameOverPanel;

    public GameObject fadePanel;

    public GameObject endLevelPanel;

    private bool hasStartedTicking = false;

    private void Start()
    {
        orbsSolved = 0;

        // Restore our persistent data tracker safely from memory RAM
        if (WavePersistenceManager.Instance != null)
        {
            currentWave = WavePersistenceManager.Instance.savedWaveNumber;
            Debug.Log($"[PERSISTENCE] Restored active gameplay state to Wave: {currentWave}");
        }
        else
        {
            currentWave = 1;
        }

        // Run the shop setup on a tiny delay so other scripts finish loading first
        StartCoroutine(DelayedShopInitialization());
    }

    private IEnumerator DelayedShopInitialization()
    {
        // Wait exactly 1 frame for all other scripts to finish their Start() sequences
        yield return null;
        InitializeWaveCycle(currentWave);
    }

    /// <summary>
    /// Freezes physics simulation and locks the current wave number safely.
    /// </summary>
    public void InitializeWaveCycle(int targetWave)
    {
        Time.timeScale = 0f;
        isInBuyTime = true;
        buyTimeRemaining = 20f;
        hasStartedTicking = false;

        currentWave = targetWave;
        Debug.Log($"[CYCLE INITIALIZED] Setting up selection phase for Wave {currentWave}");

        if (weaponSelectionPanel != null) weaponSelectionPanel.SetActive(true);

        if (selectionTimerText != null)
        {
            selectionTimerText.text = "SELECT WEAPON: 20s";
        }
    }

    private void Update()
    {
        if (isInBuyTime)
        {
            Time.timeScale = 0f;

            if (!hasStartedTicking)
            {
                hasStartedTicking = true;
                return;
            }

            buyTimeRemaining -= Time.unscaledDeltaTime;

            if (selectionTimerText != null)
            {
                selectionTimerText.text = $"{buyTimeRemaining.ToString("F2")}";
            }

            if (buyTimeRemaining <= 0)
            {
                ChooseSwordOfEuler();
            }
        }
    }

    /// <summary>
    /// Triggered directly by clicking one of your 3 Weapon Buttons!
    /// </summary>
    public void SelectWeaponBonus(string statType, int bonusValue = 5)
    {
        isInBuyTime = false;

        // Hide the panels cleanly
        if (weaponSelectionPanel != null)
        {
            weaponSelectionPanel.SetActive(false);

            Canvas panelCanvas = weaponSelectionPanel.GetComponent<Canvas>();
            if (panelCanvas != null) panelCanvas.enabled = false;
        }

        // Unfreeze time first before invoking spawner systems
        Time.timeScale = 1f;

        // Apply selected item stats directly to player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null && bonusValue > 0)
        {
            PlayerMovement2D moveScript = playerObj.GetComponent<PlayerMovement2D>();
            if (moveScript != null)
            {
                if (statType == "attack") moveScript.attackPower += bonusValue;
                else if (statType == "defense") moveScript.defense += bonusValue;
                else if (statType == "movement") moveScript.movementSpeed += (bonusValue * 0.1f);
            }
        }

        // Clear out any old elements remaining from the prior wave
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(enemy);
        foreach (GameObject orb in GameObject.FindGameObjectsWithTag("MathOrb")) Destroy(orb);

        // Update operation math patterns for the current wave
        OperationGenerator opGen = Object.FindFirstObjectByType<OperationGenerator>();
        if (opGen != null)
        {
            opGen.SetupWaveOperations(currentWave);

            if (opGen.orbSpawner != null)
            {
                opGen.orbSpawner.SpawnWaveOrbs(totalOrbsInLevel);
                Debug.Log($"[SPAWNER] Successfully generated {totalOrbsInLevel} math orbs for Wave {currentWave}!");
            }
            else
            {
                Debug.LogError("CRITICAL: The OrbSpawner slot inside your OperationGenerator script is empty!");
            }
        }

        // Safely activate monster generation scripts now that time is active
        WaveSpawner monsterSpawner = Object.FindFirstObjectByType<WaveSpawner>();
        if (monsterSpawner != null)
        {
            monsterSpawner.enabled = true;
        }

        // Initialize the main user HUD display trackers
        StartNewWave();
    }

    // FIX: Added correct 2-parameter calls to SelectWeaponBonus
    public void ChooseSwordOfEuler()    { SelectWeaponBonus("attack", 5); }
    public void ChooseArchimedesBlade() { SelectWeaponBonus("defense", 5); }
    public void ChooseGaussStaff()      { SelectWeaponBonus("movement", 5); }

    // FIX: Added missing UpdateWaveVisualLabels method
    private void UpdateWaveVisualLabels()
    {
        if (orbTrackerText != null)
            orbTrackerText.text = $"Orbs: {orbsSolved} / {totalOrbsInLevel}";

        if (waveBannerText != null)
            waveBannerText.text = currentWave == 3 ? "WAVE 3: MIXED PHASE" : $"WAVE {currentWave}";
    }

    public void StartNewWave()
    {
        orbsSolved = 0;

        if (waveBannerText != null)
        {
            waveBannerText.gameObject.SetActive(true);
            waveBannerText.text = $"WAVE {currentWave}";
        }   

        if (orbTrackerText != null)
        {
            orbTrackerText.text = $"Orbs: {orbsSolved} / {totalOrbsInLevel}";
        }

        // if (exitPortal != null) exitPortal.SetActive(false);
        if (waveCompletePanel != null) waveCompletePanel.SetActive(false);
        GameOverController.isPaused = false;
    }

    private void HideWaveBannerText()
    {
        if (waveBannerText != null && Time.timeScale > 0) waveBannerText.gameObject.SetActive(false);
    }

    public void OnOrbSolved()
    {
        orbsSolved++;

        if (orbTrackerText != null)
        {
            orbTrackerText.text = $"Orbs: {orbsSolved} / {totalOrbsInLevel}";
        }

        if (orbsSolved >= totalOrbsInLevel) WinLevel();
    }

    public void WinLevel()
    {
        Time.timeScale = 0f;

        if (waveCompletePanel != null)
        {
            waveCompletePanel.SetActive(true);

            Canvas canvasComp = waveCompletePanel.GetComponent<Canvas>();
            if (canvasComp != null) canvasComp.enabled = true;

            if (waveCompletePanel.transform.parent != null)
                waveCompletePanel.transform.parent.gameObject.SetActive(true);

            Debug.Log("[SYSTEM] Wave complete interface successfully open!");
        }

        if (exitPortal != null) exitPortal.SetActive(true);
    }

    public void LoadNextWave()
    {
        // 1. Hide the victory overlay screen cleanly before moving forward
        if (waveCompletePanel != null)
        {
            waveCompletePanel.SetActive(false);
        }

        // 2. Advance the wave loop parameters safely
        if (currentWave < 3)
        {
            currentWave++; // Safely increments instance variable from 1 -> 2
            Debug.Log($"[PROGRESSION] Advancing safely to Wave {currentWave}!");

            InitializeWaveCycle(currentWave); // Fire setup with the fresh value!
        }
        else
        {
            Debug.Log("[GAME OVER] Level 1 Completely Cleared! Transitioning to Level 2 Scene.");
            SceneManager.LoadScene("LevelTwo");
        }
    }

    public void RetryCurrentWave()
    {
        Time.timeScale = 1f;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerHealth healthScript = playerObj.GetComponent<PlayerHealth>();
            if (healthScript != null) healthScript.ResetHealthToMax();
        }

        GameObject defeatUI = GameObject.FindWithTag("UI_GameOver");
        if (defeatUI != null) defeatUI.SetActive(false);

        InitializeWaveCycle(currentWave);
    }

    public int GetSolvedCount()
    {
        return orbsSolved;
    }
}