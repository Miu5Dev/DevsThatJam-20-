using TMPro;
using UnityEngine;

public class RouletteOption : MonoBehaviour
{

    [SerializeField] private SpriteRenderer opcionSprite;
    [SerializeField] private TextMeshPro multiplier;

    [SerializeField] private int multiplierValue;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiplier.text = $"x{multiplierValue}";
    }


    public int getMultiplier()
    {
        return multiplierValue;
    }
    
}
