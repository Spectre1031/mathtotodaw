using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class NarrationController : MonoBehaviour
{
    public CharacterUISlot[] UISlots;
    public Image backgroundImage;

    public UnityEvent onDialogueEnd;
    private int dialogueIndex = -1;

    public Image characterLeft;
    public GameObject nameplateLeft;
    public TextMeshProUGUI nameplateLeftText;

    public Image characterRight;
    public GameObject nameplateRight;
    public TextMeshProUGUI nameplateRightText;

    public DialogueEntry[] dialogueEntries;

    public GameObject narrationPanel;
    public TextMeshProUGUI narrationText;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    public AudioSource sfxSource;
    public AudioClip clickSound;

    [Header("Typewriter Settings")]
    [SerializeField] private float timePerCharacter = 0.02f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullText = "";
    
    // Tracks whichever text component is active for the current layout
    private TextMeshProUGUI activeTextDisplay; 

    private IEnumerator TypeText(string textToType, TextMeshProUGUI targetDisplay)
    {
        isTyping = true;
        currentFullText = textToType;
        activeTextDisplay = targetDisplay;
        
        // Clear out the alternative box to avoid visual artifacts
        if (targetDisplay == dialogueText) narrationText.text = "";
        else dialogueText.text = "";

        // Set the full text immediately so TMP processes styling tags safely
        targetDisplay.text = currentFullText;
        targetDisplay.maxVisibleCharacters = 0;

        int totalVisibleCharacters = currentFullText.Length;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            targetDisplay.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSecondsRealtime(timePerCharacter);
        }

        isTyping = false;
    }

    private void SkipToFullText()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        if (activeTextDisplay != null)
        {
            activeTextDisplay.maxVisibleCharacters = currentFullText.Length;
            activeTextDisplay.text = currentFullText;
        }
        
        isTyping = false;
    }

    void Start()
    {
        NextDialogue();
    }

    void ShowRightCharacter(Character character)
    {
        nameplateRightText.text = character.name;
        nameplateRight.SetActive(true);
        characterRight.sprite = character.sprite;
    }
    
    void HideRightCharacter()
    {
        nameplateRight.SetActive(false);
    }

    void DisableCharacter()
    {
        narrationPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        characterLeft.enabled = false;
        characterRight.enabled = false;
        nameplateLeft.SetActive(false);
        nameplateRight.SetActive(false);
    }

    void EnableCharacter()
    {
        dialoguePanel.SetActive(true);
        characterLeft.enabled = true;
        characterRight.enabled = true;
        narrationPanel.SetActive(false);
    }

    void ShowLeftCharacter(Character character)
    {
        nameplateLeftText.text = character.name;
        nameplateLeft.SetActive(true);
        characterLeft.sprite = character.sprite;
    }

    void HideLeftCharacter()
    {
        nameplateLeft.SetActive(false);
    }

    public void NextDialogue()
    {
        Debug.Log("Next");

        if (sfxSource != null && clickSound != null && dialogueIndex > -1)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        if (isTyping)
        {
            SkipToFullText();
            return;
        }

        if(dialogueIndex < dialogueEntries.Length - 1)
        {
            dialogueIndex++;
            Debug.Log("dialogue index: " + dialogueIndex + "/" + dialogueEntries.Length);

            var current = dialogueEntries[dialogueIndex];
            
            // 1. Determine layout panels first before starting the stream
            CharacterUISlot leftSlot = UISlots[0];
            CharacterUISlot rightSlot = UISlots[1];
            TextMeshProUGUI chosenDisplayComponent = dialogueText; // Fallback default
            
            if(backgroundImage.sprite != current.background)
            {
                backgroundImage.sprite = current.background;   
            }

            switch (current.layout)
            {
                case DialogueLayout.Text:
                    DisableCharacter();
                    chosenDisplayComponent = narrationText;
                    break;

                case DialogueLayout.SoloLeft:
                    EnableCharacter();
                    ShowLeftCharacter(current.leftCharacter);
                    leftSlot.LoadCharacter(current.leftCharacter);
                    HideRightCharacter();
                    break;

                case DialogueLayout.SoloRight:
                    EnableCharacter();
                    ShowRightCharacter(current.rightCharacter);
                    rightSlot.LoadCharacter(current.rightCharacter);
                    HideLeftCharacter();
                    break;

                case DialogueLayout.Dual:   
                    EnableCharacter();
                    ShowLeftCharacter(current.leftCharacter);
                    leftSlot.LoadCharacter(current.leftCharacter);
                    ShowRightCharacter(current.rightCharacter);
                    rightSlot.LoadCharacter(current.rightCharacter);
                    break;
            }

            // 2. Fire the typewriter animation targeted to the correct UI component
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(current.text, chosenDisplayComponent));

            // 3. Update character tracking frames
            foreach (var slot in UISlots)
            {
                if (current.speaker != null)
                {
                    slot.UpdateFocus(current.speaker);
                }
                else
                {
                    // Safe cleanup if narration layout has no active speaker
                    slot.UpdateFocus(null); 
                }
            }
        } 
        else 
        {
            nextButton.interactable = false;
            onDialogueEnd.Invoke();
        }
    }
}