using TMPro;
using UnityEngine;

public class RouletteOption : MonoBehaviour
{
    public SpriteRenderer opcionSprite;
    public TextMeshPro multiplier;
    public string sceneName;
    public int multiplierValue;

    void Start()
    {
        updateDisplay();
    }

    // Método para inicializar desde MinigameData
    public void Initialize(MinigameData data, int multiplier)
    {
        opcionSprite = GetComponentInChildren<SpriteRenderer>(); // Re-fetch every time
        opcionSprite.sprite = data.sprite;
        sceneName = data.sceneName;
        multiplierValue = multiplier;
        updateDisplay();
    }

    public void updateDisplay()
    {
        multiplier.text = $"x{multiplierValue}";
    }

    public int getMultiplier()
    {
        return multiplierValue;
    }
}