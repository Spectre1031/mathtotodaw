using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]
public class Character : ScriptableObject
{
    public new string name; // Added 'new' keyword here
    public Sprite sprite;
}