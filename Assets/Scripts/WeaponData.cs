using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string weaponName;
    public string targetStat;
    public int boostValue;
    public Sprite weaponSprite;
}

public class WeaponData : MonoBehaviour
{
    public Weapon swordOfEuler;
    public Weapon archimedesBlade;
    public Weapon gaussStaff;

    void Awake()
    {
        // Setup values exactly from your GDD Page 14!
        swordOfEuler.weaponName = "Sword of Euler";
        swordOfEuler.targetStat = "attack";
        swordOfEuler.boostValue = 9; // [cite: 281, 283, 284]

        archimedesBlade.weaponName = "Archimedes' Blade";
        archimedesBlade.targetStat = "defense";
        archimedesBlade.boostValue = 7; // [cite: 285, 286, 287]

        gaussStaff.weaponName = "Gauss's Staff";
        gaussStaff.targetStat = "movement";
        gaussStaff.boostValue = 8; // [cite: 288, 289, 290]
    }
}