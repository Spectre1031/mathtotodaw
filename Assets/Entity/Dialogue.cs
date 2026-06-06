using UnityEngine;
using UnityEngine.UI;

public enum DialogueLayout { SoloLeft, SoloRight, Dual, Text }
[System.Serializable]
public class DialogueEntry
{
    public DialogueLayout layout;
    public Character leftCharacter;
    public Character rightCharacter;
    public Character speaker;
    public Sprite background;
    [TextArea(3, 10)]
    public string text;
}