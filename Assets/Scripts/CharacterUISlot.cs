using UnityEngine;
using UnityEngine.UI;

public class CharacterUISlot : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    private Animator animator;
    private Character currentResident;

    void Awake() => animator = GetComponent<Animator>();

    public void LoadCharacter(Character newCharacter)
    {
        currentResident = newCharacter;
        portraitImage.sprite = newCharacter.sprite;
        Debug.Log($"Loaded character {newCharacter.name} into slot {gameObject.name}");
        animator.SetBool("isTalking", true);
        Debug.Log($"Set isTalking to true for {gameObject.name} when loading {newCharacter.name}");
    }

    public void UpdateFocus(Character activeSpeaker)
    {
        if (animator == null)
        {
            Debug.LogError($"Animator component is missing on {gameObject.name}!");
            return;
        }

        bool isSpeaking = (currentResident == activeSpeaker);
        animator.SetBool("isTalking", isSpeaking);
        
        
}
}