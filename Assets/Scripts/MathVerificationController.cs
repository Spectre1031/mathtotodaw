using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MathVerificationController : MonoBehaviour
{
    [Header("UI Element References")]
    public GameObject verificationPanel;
    public TMP_Text expressionText;
    public TMP_InputField answerInputField;
    public Animator questionPanel;

    private MathOrb activeOrbReference;

    void Start()
    {
        if (verificationPanel != null) verificationPanel.SetActive(false);
    }

    public void OpenQuestionWindow(MathOrb orb)
    {
        // Time.timeScale = 0f; // Freeze the game simulation loop
        GameOverController.isAnswering = true;
        activeOrbReference = orb;
        Debug.Log("OPEN");
        if (verificationPanel != null) verificationPanel.SetActive(true);
        questionPanel.SetTrigger("show");
        if (expressionText != null) expressionText.text = $"{orb.op1} {orb.op} {orb.op2} = ?";
        if (answerInputField != null)
        {
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }
    }

    public void OnSubmitAnswer()
    {
        if (activeOrbReference == null) return;

        int playerAnswer;
        if (int.TryParse(answerInputField.text, out playerAnswer))
        {
            if (playerAnswer == activeOrbReference.res)
            {
                Debug.Log("CORRECT! Distributing stat boost.");

                // ==========================================
                // SUCCESS ROUTE: REWARD DISTRIBUTION
                // ==========================================
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    string statToBoost = activeOrbReference.boost.stat.ToLower();
                    int boostAmount = activeOrbReference.boost.value;

                    if (statToBoost == "heal" || statToBoost == "maxhealth")
                    {
                        PlayerHealth healthScript = playerObj.GetComponent<PlayerHealth>();
                        if (healthScript != null) healthScript.ApplyHealthBoost(statToBoost, boostAmount);
                    }
                    else if (statToBoost == "attack" || statToBoost == "movement" || statToBoost == "speed")
                    {
                        PlayerMovement2D movementScript = playerObj.GetComponent<PlayerMovement2D>();
                        if (movementScript != null) movementScript.ApplyCombatBoost(statToBoost, boostAmount);
                    }
                }

                // ==========================================
                // LEVEL PROGRESSION ROUTE: ORB TRACKER UPDATE
                // ==========================================
                LevelManager lvlManager = Object.FindFirstObjectByType<LevelManager>();
                if (lvlManager != null)
                {
                    lvlManager.OnOrbSolved();
                }

                // Cleanup and conditional pause management
                questionPanel.SetTrigger("correct");
                Destroy(activeOrbReference.gameObject);

                if (lvlManager != null && lvlManager.GetSolvedCount() < lvlManager.totalOrbsInLevel)
                {
                    GameOverController.isAnswering = false;
                }
            }
            else
            {
                Debug.Log("WRONG ANSWER! Triggering health penalty.");
                questionPanel.SetTrigger("shake");
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    PlayerHealth healthScript = playerObj.GetComponent<PlayerHealth>();
                    if (healthScript != null)
                    {
                        int penaltyDamage = 15;
                        healthScript.TakeDamage(penaltyDamage);
                    }
                }

                //if (expressionText != null) expressionText.text = "<color=red>INCORRECT!</color> Try again:";
                answerInputField.text = "";
                answerInputField.ActivateInputField();
            }
        }
    }

    public void CloseWindow()
    {
        if (verificationPanel != null) verificationPanel.SetActive(false);
        activeOrbReference = null;

        LevelManager lvlManager = Object.FindFirstObjectByType<LevelManager>();
        if (lvlManager != null && lvlManager.GetSolvedCount() < lvlManager.totalOrbsInLevel)
        {
            Time.timeScale = 1f;
        }
    }
}