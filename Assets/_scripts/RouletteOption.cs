using TMPro;
using UnityEngine;

public class RouletteOption : MonoBehaviour
{
    public SpriteRenderer opcionSprite;
    public TextMeshPro multiplier;
    public string sceneName;
    public int multiplierValue;
    
    public AudioClip backgroundMusic;
    
    [Header("Sprite Size (Fallback)")]
    [SerializeField] private Vector2 defaultSpriteSize = new Vector2(2f, 2f); // ✅ Valor por defecto
    [SerializeField] private bool defaultMaintainAspectRatio = true;

    void Start()
    {
        updateDisplay();
    }

    // Método para inicializar desde MinigameData
    public void Initialize(MinigameData data, int multiplier)
    {
        opcionSprite = GetComponentInChildren<SpriteRenderer>();
        opcionSprite.sprite = data.sprite;
        sceneName = data.sceneName;
        multiplierValue = multiplier;
        backgroundMusic = data.backgroundMusic;
        
        // ✅ Usa el tamaño del ScriptableObject
        AdjustSpriteSize(data.spriteSize, data.maintainAspectRatio);
        
        updateDisplay();
    }

    // ✅ Método sobrecargado que acepta parámetros del ScriptableObject
    void AdjustSpriteSize(Vector2 targetSize, bool maintainAspect)
    {
        if (opcionSprite == null || opcionSprite.sprite == null) return;

        // Obtiene el tamaño actual del sprite en unidades
        Vector2 spriteSize = opcionSprite.sprite.bounds.size;

        if (maintainAspect)
        {
            // Escala uniforme basada en el ancho
            float scale = targetSize.x / spriteSize.x;
            opcionSprite.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            // Escala no uniforme (puede deformar)
            float scaleX = targetSize.x / spriteSize.x;
            float scaleY = targetSize.y / spriteSize.y;
            opcionSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }

    // ✅ Versión pública por si quieres ajustar el tamaño manualmente
    public void SetSpriteSize(Vector2 size, bool maintainAspect = true)
    {
        AdjustSpriteSize(size, maintainAspect);
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
