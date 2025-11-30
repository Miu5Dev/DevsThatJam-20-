using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseOpenerManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject mainView;          // Panel con las 3 cajas
    [SerializeField] private GameObject rouletteView;      // Panel de la ruleta
    [SerializeField] private CanvasGroup fadePanel;        // Panel negro (CanvasGroup)
    [SerializeField] private Image targetColorIndicator;   // Imagen arriba derecha

    [Header("Cajas")]
    [SerializeField] private CaseBox[] caseBoxes;          // 3 cajas en escena

    [Header("Ruleta")]
    [SerializeField] private RouletteScroller rouletteScroller;

    [Header("Skins disponibles")]
    [SerializeField] private List<WeaponSkin> availableSkins;

    [Header("Config")]
    [SerializeField] private int maxCases = 3;

    private RarityColor targetColor;
    private int casesOpened = 0;
    private bool isRunning = false;

    private void Start()
    {
        SetupCases();
        StartNewRound();
    }

    private void SetupCases()
    {
        foreach (var box in caseBoxes)
        {
            box.OnCaseClicked += HandleCaseClick;
        }
    }

    private void StartNewRound()
    {
        casesOpened = 0;
        isRunning = false;

        // Activamos vista de cajas, ocultamos ruleta
        mainView.SetActive(true);
        rouletteView.SetActive(false);

        // Reset fade
        fadePanel.alpha = 0f;

        // Habilitar clic en todas las cajas
        foreach (var box in caseBoxes)
        {
            box.EnableClick(true);
        }

        DetermineTargetColor();
    }

    private void DetermineTargetColor()
    {
        // Si no tienes GameManager aún, prueba con valor fijo
        float multiplier = GameManager.Instance != null 
            ? GameManager.Instance.minigameMuliplier 
            : 1f;

        targetColor = CalculateTargetFromMultiplier(multiplier);
        targetColorIndicator.color = GetColorFromRarity(targetColor);
    }

    private RarityColor CalculateTargetFromMultiplier(float mult)
    {
        if (mult >= 10f) return RarityColor.Legendary;
        if (mult >= 5f)  return RarityColor.Epic;
        if (mult >= 2.5f) return RarityColor.Rare;
        if (mult >= 1.5f) return RarityColor.Uncommon;
        return RarityColor.Common;
    }

    private void HandleCaseClick(CaseBox box)
    {
        if (isRunning) return; // Evita doble disparo mientras anima

        isRunning = true;
        StartCoroutine(OpenCaseSequence(box));
    }

    private IEnumerator OpenCaseSequence(CaseBox box)
    {
        // Deshabilito solo esta caja (ya usada)
        box.EnableClick(false);

        // Fade a negro
        yield return FadeToBlack(0.3f);

        // Cambiar a vista ruleta
        mainView.SetActive(false);
        rouletteView.SetActive(true);

        // Fade desde negro
        yield return FadeFromBlack(0.2f);

        // Generar resultado
        WeaponSkin result = GenerateResult();

        // Correr ruleta (IEnumerator)
        yield return rouletteScroller.StartRoulette(availableSkins, result);

        // Espera un poco para que el jugador vea el resultado
        yield return new WaitForSeconds(1.5f);

        casesOpened++;

        bool success = (int)result.rarity >= (int)targetColor;

        if (success)
        {
            // Ganó: termina el minijuego
            EndMinigame(true, result);
            yield break;
        }

        if (casesOpened >= maxCases)
        {
            // Se acabaron las cajas, perdió
            EndMinigame(false, result);
            yield break;
        }

        // Si aún hay cajas y no acertó, volvemos a la vista de cajas
        yield return FadeToBlack(0.3f);
        rouletteView.SetActive(false);
        mainView.SetActive(true);
        yield return FadeFromBlack(0.3f);

        isRunning = false;
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        fadePanel.alpha = 0f;
    }

    private WeaponSkin GenerateResult()
    {
        float total = 0f;
        foreach (var skin in availableSkins)
            total += skin.dropChance;

        float random = Random.Range(0f, total);
        float current = 0f;

        foreach (var skin in availableSkins)
        {
            current += skin.dropChance;
            if (random <= current)
                return skin;
        }

        // fallback
        return availableSkins.Count > 0 ? availableSkins[0] : null;
    }

    private Color GetColorFromRarity(RarityColor rarity)
    {
        return rarity switch
        {
            RarityColor.Common    => new Color(0.7f, 0.7f, 0.7f),
            RarityColor.Uncommon  => new Color(0.3f, 0.5f, 1f),
            RarityColor.Rare      => new Color(0.5f, 0.3f, 1f),
            RarityColor.Epic      => new Color(1f, 0.3f, 0.8f),
            RarityColor.Legendary => new Color(1f, 0.8f, 0.2f),
            _ => Color.white
        };
    }

    private void EndMinigame(bool success, WeaponSkin finalResult)
    {
        Debug.Log(success
            ? $"GANASTE: {finalResult?.skinName} ({finalResult?.rarity})"
            : "PERDISTE: no conseguiste el color requerido.");

        GameManager.Instance.endMinigame(success);        
        
    }
}
