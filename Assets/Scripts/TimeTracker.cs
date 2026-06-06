using UnityEngine;
using System;

public class TimeTracker : MonoBehaviour
{
    public static TimeTracker instance { get; private set; }

    [Header("Wave Time Settings")]
    public float timeRemaining = 90f; // This is your dynamic Editor value!
    
    // Hidden backup variable to store whatever you typed in the editor
    private float maxWaveTime; 
    private bool isTimerRunning = false;

    private LevelManager lvlManager;

    public static event Action<float> OnTimeUpdated; 
    public static event Action OnTimeOut; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // 1. Snapshot the exact value you set in the Unity Editor at launch!
        maxWaveTime = timeRemaining; 

        lvlManager = UnityEngine.Object.FindFirstObjectByType<LevelManager>();
        isTimerRunning = true;
    }

    // Dynamic Reset Function
    public static void ResetTimer()
    {
        if (instance == null)
        {
            instance = UnityEngine.Object.FindFirstObjectByType<TimeTracker>();
        }

        if (instance != null)
        {
            // 2. Restore the clock using our saved snapshot value!
            instance.timeRemaining = instance.maxWaveTime; 
            instance.isTimerRunning = true; 
            
            Debug.Log($"[TIMER RESET] Clock restored to Editor baseline: {instance.timeRemaining} seconds.");
        }
        else
        {
            Debug.LogError("CRITICAL: Tried to ResetTimer, but no active TimeTracker script was found in the scene!");
        }
    }

    public static void BoostTime(int boost)
    {
        if (instance == null)
        {
            instance = UnityEngine.Object.FindFirstObjectByType<TimeTracker>();
        }

        if (instance != null)
        {
            instance.timeRemaining += boost;
            Debug.Log($"[TIMER] Added {boost} seconds. New time: {instance.timeRemaining}");
        }
    }

    void Update()
    {
        if (lvlManager != null && lvlManager.weaponSelectionPanel != null && lvlManager.weaponSelectionPanel.activeSelf)
        {
            return; 
        }

        if (!GameOverController.isPaused)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                OnTimeUpdated?.Invoke(timeRemaining); 
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                isTimerRunning = false;
                OnTimeOut?.Invoke();
            }
        }
    }
}