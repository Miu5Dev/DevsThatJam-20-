using UnityEngine;

[CreateAssetMenu(fileName = "MinigameData", menuName = "Scriptable Objects/MinigameData")]
public class MinigameData : ScriptableObject
{
    public string minigameName;
    public Sprite sprite;
    public string sceneName;
    [Tooltip("Rango de multiplicadores posibles para este minijuego")]
    public Vector2Int multiplierRange = new Vector2Int(1, 5);
}
