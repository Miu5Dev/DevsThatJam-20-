using UnityEngine;

[CreateAssetMenu(fileName = "MinigameData", menuName = "Minijuegos/MinigameData")]
public class MinigameData : ScriptableObject
{
    public string minigameName;
    public Sprite sprite;
    public string sceneName;

    public AudioClip backgroundMusic;
    
    [Tooltip("Rango de multiplicadores posibles para este minijuego")]
    public Vector2Int multiplierRange = new Vector2Int(1, 5);
    
    [Header("Sprite Display")]
    [Tooltip("Tamaño del sprite en la ruleta (en unidades de Unity)")]
    public Vector2 spriteSize = new Vector2(2f, 2f); // ✅ Tamaño personalizable
    
    [Tooltip("Mantener proporción del sprite o estirar al tamaño exacto")]
    public bool maintainAspectRatio = true; // ✅ Opción de aspect ratio
}