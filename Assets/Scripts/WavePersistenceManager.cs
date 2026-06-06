using UnityEngine;

public class WavePersistenceManager : MonoBehaviour
{
    public static WavePersistenceManager Instance { get; private set; }

    [Header("Persistent Progression Tracker")]
    public int savedWaveNumber = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // 📢 AUTOMATIC ROOT FIX: Detaches this object from any parent container instantly!
            transform.SetParent(null);

            DontDestroyOnLoad(gameObject); // Now this is 100% guaranteed to work!
            Debug.Log("[PERSISTENCE] Manager detached to root and locked in global RAM memory.");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}