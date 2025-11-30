using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Economy")]
    [SerializeField] private int coins = 100;

    [Header("Minigame Settings")]
    [SerializeField] private int coinsRequired;
    [SerializeField] private int rounds;
    [SerializeField] public int minigameMuliplier;
    
    [Header("Current Values")]
    [SerializeField] private int currentRound;
    
    [Header("Posible Minigames")]
    [SerializeField] private List<MinigameData> minigamesData = new List<MinigameData>();
    [SerializeField] public RuletaPhysics currentRoulette;
    
    [Header("Controllers")]
    [SerializeField] public LobbyController _lobbyController;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Registrar evento
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Limpiar evento para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se llama automáticamente cada vez que se carga una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-encontrar referencias de la nueva escena
        RefreshSceneReferences();
        
        // Solo poblar si estamos en PlayingZone
        if (scene.name == "PlayingZone")
        {
            populateRoulette();
            
            // Re-suscribir al evento de la nueva ruleta
            if (currentRoulette != null)
            {
                currentRoulette.onSpinComplete.RemoveListener(OnResultadoRuleta);
                currentRoulette.onSpinComplete.AddListener(OnResultadoRuleta);
            }
        }
    }

    // Método para re-encontrar todas las referencias de escena
    private void RefreshSceneReferences()
    {
        currentRoulette = FindFirstObjectByType<RuletaPhysics>();
    }

    public void endMinigame(bool winned)
    {
        if (!winned)
            onLose();   
        else
            onWin();
    
        // Iniciar coroutine para cargar escena y mostrar título después
        StartCoroutine(EndMinigameSequence(winned));
    }

    private IEnumerator EndMinigameSequence(bool winned)
    {
        // Cargar la escena
        SceneManager.LoadScene("PlayingZone");
    
        // Esperar un frame para que la escena se cargue y el LobbyController se registre
        yield return new WaitForEndOfFrame();
    
        // Esperar un frame adicional para asegurar que Start() y OnEnable() se ejecuten
        yield return new WaitForEndOfFrame();
    
        // Ahora sí, mostrar el título
        if (_lobbyController != null)
        {
            _lobbyController.DisplayTitle(winned);
        }
        else
        {
            Debug.LogError("GameManager: LobbyController no está registrado después de cargar la escena!");
        }
    }

    public void onWin()
    {
        if (minigameMuliplier == 1)
        {
            coins+=coins;
        }
        else
            coins *= minigameMuliplier;
    }

    public void onLose()
    {
        if (minigameMuliplier == 1)
        {
            coins-=coins;
        }
        else
            coins /= minigameMuliplier;
    }

    // Callback de la ruleta
    private void OnResultadoRuleta(int resultado)
    {
        Debug.Log(resultado-1);
        RouletteOption selectedOption = currentRoulette.options[resultado-1];
        minigameMuliplier = selectedOption.multiplierValue;
        SceneManager.LoadScene(selectedOption.sceneName);
    }

    // Población de opciones de ruleta
    private void populateRoulette()
    {
        if (minigamesData.Count == 0 || currentRoulette == null) return;

        foreach (var option in currentRoulette.options)
        {
            if (option != null) // Verificación de seguridad
            {
                MinigameData randomMinigame = minigamesData[Random.Range(0, minigamesData.Count)];
                
                int randomMultiplier = Random.Range(randomMinigame.multiplierRange.x, 
                    randomMinigame.multiplierRange.y + 1);
                
                option.Initialize(randomMinigame, randomMultiplier);
            }
        }
    }

    public int getCoins()
    {
        return coins;
    }
    

    public void spinWheel()
    {
        if (currentRoulette == null)
            currentRoulette = FindFirstObjectByType<RuletaPhysics>();
            
        currentRoulette?.SpinWheel();
    }
}
