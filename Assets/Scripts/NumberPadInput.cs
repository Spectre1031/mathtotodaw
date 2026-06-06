using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberPadInput : MonoBehaviour
{
    [Header("Target Components")]
    public TMP_InputField targetInputField;
    public Button originalSubmitButton;

    public void AppendDigit(string digit)
    {
        Debug.Log($"[KEYPAD CLICK] Directly injecting: {digit}");

        if (targetInputField != null)
        {
            // 1. If there's an active placeholder, hide it immediately since we are typing
            if (targetInputField.placeholder != null && targetInputField.placeholder.gameObject.activeSelf)
            {
                targetInputField.placeholder.gameObject.SetActive(false);
            }

            // 2. BYPASS UNITY'S INPUT SYSTEM: Grab the raw internal text mesh string and force the character in!
            if (targetInputField.textComponent != null)
            {
                // Sync the logical value and the visual mesh at the exact same time
                targetInputField.text += digit;
                targetInputField.textComponent.text = targetInputField.text;

                // Force an immediate graphics card repaint of the text characters
                targetInputField.textComponent.ForceMeshUpdate();
            }
        }
        else
        {
            Debug.LogError("NumberPadInput script is missing the Target Input Field reference!");
        }
    }

    public void SubmitAnswer()
    {
        if (originalSubmitButton != null)
        {
            originalSubmitButton.onClick.Invoke();
        }
    }

    public void Backspace()
    {
        if (targetInputField != null && targetInputField.text.Length > 0)
        {
            // Remove the last character logically
            targetInputField.text = targetInputField.text.Substring(0, targetInputField.text.Length - 1);

            // Force the visual text mesh to match instantly
            if (targetInputField.textComponent != null)
            {
                targetInputField.textComponent.text = targetInputField.text;
                targetInputField.textComponent.ForceMeshUpdate();
            }

            // Bring the placeholder back if the field is completely empty
            if (targetInputField.text.Length == 0 && targetInputField.placeholder != null)
            {
                targetInputField.placeholder.gameObject.SetActive(true);
            }
        }
    }
}