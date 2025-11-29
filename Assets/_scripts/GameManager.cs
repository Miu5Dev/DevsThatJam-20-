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
    
