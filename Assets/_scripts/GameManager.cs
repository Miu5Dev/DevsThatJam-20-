<<<<<<< Updated upstream
using System.Collections.Generic;
=======
﻿using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
<<<<<<< Updated upstream
    public static GameManager Instance;

    [Header("Player Economy")]
    [SerializeField] private int monedas = 100;
    [SerializeField] private int apuestaActual = 100;
    
    [Header("Minigame Settings")]
=======
    [Header("Requirements")]
>>>>>>> Stashed changes
    [SerializeField] private int coinsRequired;
    [SerializeField] private int rounds;
    [SerializeField] private int minigameMuliplier;
    
    [Header("Current Values")]
    [SerializeField] private int currentRound;
<<<<<<< Updated upstream
    
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
=======
    [SerializeField] private int currentCoins;
    
    [Header("Posible Minigames")]
    [SerializeField] private List<MinigameData> minigamesData = new List<MinigameData>();

    [SerializeField] private RuletaPhysics currentRoulette;
>>>>>>> Stashed changes

    private void Start()
    {
        populateRoulette();
<<<<<<< Updated upstream
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
=======
        
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

>>>>>>> Stashed changes
    private void OnResultadoRuleta(int resultado)
    {
        RouletteOption selectedOption = currentRoulette.options[resultado];
        minigameMuliplier = selectedOption.multiplierValue;
        SceneManager.LoadScene(selectedOption.sceneName);
    }

<<<<<<< Updated upstream
    // Población de opciones de ruleta
=======
>>>>>>> Stashed changes
    private void populateRoulette()
    {
        if (minigamesData.Count == 0) return;

        foreach (var option in currentRoulette.options)
        {
            MinigameData randomMinigame = minigamesData[Random.Range(0, minigamesData.Count)];
            
            int randomMultiplier = Random.Range(randomMinigame.multiplierRange.x, 
                randomMinigame.multiplierRange.y + 1);
            
<<<<<<< Updated upstream
            option.Initialize(randomMinigame, randomMultiplier);
        }
    }
}
=======
            // Inicializar la opción
            option.Initialize(randomMinigame, randomMultiplier);
        }
    }
}
>>>>>>> Stashed changes
