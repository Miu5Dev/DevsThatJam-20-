using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Economy")]
    [SerializeField] private int monedas = 100;
    [SerializeField] private int apuestaActual = 100;
    
    [Header("Minigame Settings")]
    [SerializeField] private int coinsRequired;
    [SerializeField] private int rounds;
    [SerializeField] private int minigameMuliplier;
    
    [Header("Current Values")]
    [SerializeField] private int currentRound;
    
    [Header("Posible Minigames")]
    [SerializeField] private List<MinigameData> minigamesData = new List<MinigameData>();
    [SerializeField] private RuletaPhysics currentRoulette;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        populateRoulette();
        currentRoulette.SpinWheel();
    }

    // Sistema de apuestas
    public bool EmpezarMinijuego(int apuesta)
    {
        if (monedas < apuesta)
            return false;
        
        monedas -= apuesta;
        apuestaActual = apuesta;
        return true;
    }

    // Callback cuando termina el minijuego
    public void onMinigameEnded(bool winned)
    {
        if (!winned)
            PerdisteMinijuego();
        else
            GanasteMinijuego(minigameMuliplier);
    }

    public void GanasteMinijuego(int multiplicador)
    {
        int ganancia = apuestaActual * multiplicador;
        monedas += ganancia;
        apuestaActual = 0;
        Debug.Log("YOU WIN: +" + ganancia + " coins. Total: " + monedas);
    }

    public void PerdisteMinijuego()
    {
        Debug.Log("YOU LOSE " + apuestaActual + " coins. Total: " + monedas);
        apuestaActual = 0;
    }

    // Callback de la ruleta
    private void OnResultadoRuleta(int resultado)
    {
        RouletteOption selectedOption = currentRoulette.options[resultado];
        minigameMuliplier = selectedOption.multiplierValue;
        SceneManager.LoadScene(selectedOption.sceneName);
    }

    // Población de opciones de ruleta
    private void populateRoulette()
    {
        if (minigamesData.Count == 0) return;

        foreach (var option in currentRoulette.options)
        {
            MinigameData randomMinigame = minigamesData[Random.Range(0, minigamesData.Count)];
            
            int randomMultiplier = Random.Range(randomMinigame.multiplierRange.x, 
                randomMinigame.multiplierRange.y + 1);
            
            option.Initialize(randomMinigame, randomMultiplier);
        }
    }
}
