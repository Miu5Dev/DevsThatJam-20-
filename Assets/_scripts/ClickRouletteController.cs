using UnityEngine;
using TMPro;

public class ClickRouletteController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject caja;          
    public GameObject ruletaUI;      
    public TextMeshProUGUI rouletteText; 

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip clickClip;

    [Header("Multiplicadores y probabilidades")]
    public float[] multipliers = { 1f, 2f, 3f, 5f, 10f };
    public float[] weights = { 40f, 30f, 20f, 8f, 2f }; 

    [Header("Animación ruleta")]
    public float shuffleDuration = 1.0f;   
    public float shuffleInterval = 0.05f;  

    private bool _isRolling = false;

    void Start()
    {
        if (ruletaUI != null)
            ruletaUI.SetActive(false);  
    }

    void OnMouseDown()
    {
        if (_isRolling) return; 

        // 1) Sonido
        if (audioSource != null && clickClip != null)
            audioSource.PlayOneShot(clickClip);

        // 2) Ocultar caja
        //if (caja != null)
        //    caja.SetActive(false);

        // 3) Mostrar ruleta y empezar animación
        if (ruletaUI != null)
            ruletaUI.SetActive(true);

        StartCoroutine(RouletteRoutine());
    }

    System.Collections.IEnumerator RouletteRoutine()
    {
        _isRolling = true;

        float elapsed = 0f;

        // Fase de “giro”: cambia números rápido
        while (elapsed < shuffleDuration)
        {
            elapsed += shuffleInterval;

            // elige un multiplicador al azar solo para el efecto visual
            int randomIndex = Random.Range(0, multipliers.Length);
            float tempMult = multipliers[randomIndex];

            if (rouletteText != null)
                rouletteText.text = "x" + tempMult.ToString("0");

            yield return new WaitForSeconds(shuffleInterval);
        }

        // Resultado final usando probabilidades (weights)
        float finalMult = GetWeightedRandomMultiplier();

        if (rouletteText != null)
            rouletteText.text = "x" + finalMult.ToString("0");

        Debug.Log("RESULTADO FINAL RUETA -> x" + finalMult);

        _isRolling = false;

        // Aquí puedes aplicar el multiplicador a tus monedas
        // GameManager.Instance.ApplyMultiplier(finalMult);
    }

    float GetWeightedRandomMultiplier()
    {
        if (multipliers.Length == 0 || weights.Length != multipliers.Length)
        {
            // Por seguridad, si algo está mal, devolvemos x1
            return 1f;
        }

        // Suma total de pesos
        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];

        float randomValue = Random.Range(0f, totalWeight); // [0, totalWeight) [web:79][web:82]

        float cumulative = 0f;
        for (int i = 0; i < multipliers.Length; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
            {
                return multipliers[i];
            }
        }

        // fallback
        return multipliers[multipliers.Length - 1];
    }
}
