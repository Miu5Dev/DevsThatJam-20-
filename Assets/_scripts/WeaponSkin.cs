using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponSkin", menuName = "CaseOpener/Skin")]
public class WeaponSkin : ScriptableObject
{
    public string skinName;
    public Sprite skinSprite;
    public RarityColor rarity;
    public float dropChance;
}

public enum RarityColor 
{ 
    Common, 
    Uncommon, 
    Rare, 
    Epic, 
    Legendary 
}