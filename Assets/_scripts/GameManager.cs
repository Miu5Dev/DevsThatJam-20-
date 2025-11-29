<<<<<<< Updated upstream
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int monedas = 100;
    public int apuestaActual = 100;

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
    public bool EmpezarMinijuego(int apuesta)
    {
        if (monedas < apuesta)
            return false;
        monedas -= apuesta;
        apuestaActual = apuesta;
        return true;
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
}
    
=======
﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private int coinsRequired;
    [SerializeField] private int rounds;
    [SerializeField] private int minigameMuliplier;
    
    [Header("Current Values")]
    [SerializeField] private int currentRound;
    [SerializeField] private int currentCoins;
    
    [Header("Posible Minigames")]
    [SerializeField] private List<MinigameData> minigamesData = new List<MinigameData>();

    [SerializeField] private RuletaPhysics currentRoulette;

    private void Start()
    {
        populateRoulette();
        
        currentRoulette.SpinWheel();
    }

    public void onMinigameEnded(bool winned)
    {
        if (!winned)
            onLose();
        else
            onWin();
    }

    private void onWin()
    {
        currentCoins *= minigameMuliplier;
    }

    private void onLose()
    {
        currentCoins /= minigameMuliplier;
    }

    private void OnResultadoRuleta(int resultado)
    {
        RouletteOption selectedOption = currentRoulette.options[resultado];
        minigameMuliplier = selectedOption.multiplierValue;
        SceneManager.LoadScene(selectedOption.sceneName);
    }

    private void populateRoulette()
    {
        if (minigamesData.Count == 0) return;

        foreach (var option in currentRoulette.options)
        {
            MinigameData randomMinigame = minigamesData[Random.Range(0, minigamesData.Count)];
            
            int randomMultiplier = Random.Range(randomMinigame.multiplierRange.x, 
                randomMinigame.multiplierRange.y + 1);
            
            // Inicializar la opción
            option.Initialize(randomMinigame, randomMultiplier);
        }
    }
}
>>>>>>> Stashed changes
