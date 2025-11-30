using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [Header("Title Display")]
    [SerializeField] private GameObject titleHolder;
    [SerializeField] private TMP_Text titleTitle;
    [SerializeField] private TMP_Text titleSubtitle;


    [SerializeField] private GameObject gameoverpanel;
    
    [Header("Player Stats")]
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text roundStatus;
    [SerializeField] private TMP_Text requiredMoney;
    
    
    private int cachedMoney = -1;
    [SerializeField] private Canvas mainCanvas;
    private RectTransform titleHolderRect;
    
    // Referencia a la coroutine activa para evitar conflictos
    private Coroutine activeAnimationCoroutine;

    private void Start()
    {
        GameManager.Instance._lobbyController = this;
        
        // Encontrar el Canvas principal
        mainCanvas = FindObjectOfType<Canvas>();
        
        // Re-encontrar referencias si están null
        ValidateReferences();
        
        // Obtener el RectTransform del titleHolder
        if (titleHolder != null)
        {
            titleHolderRect = titleHolder.GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        // Si las referencias están perdidas después de scene reload
        if (titleHolder == null || titleTitle == null || titleSubtitle == null)
        {
            Debug.LogWarning("LobbyController: Referencias perdidas. Buscando en Canvas...");
            
            if (mainCanvas == null)
                mainCanvas = FindObjectOfType<Canvas>();
            
            if (mainCanvas != null)
            {
                Transform canvasTransform = mainCanvas.transform;
                
                // Buscar TitlePanel dentro del Canvas
                titleHolder = FindChildByName(canvasTransform, "TitlePanel");
                
                if (titleHolder != null)
                {
                    // Obtener el RectTransform
                    titleHolderRect = titleHolder.GetComponent<RectTransform>();
                    
                    // Buscar Title y Subtitle dentro de TitlePanel
                    titleTitle = titleHolder.transform.Find("Title")?.GetComponent<TMP_Text>();
                    titleSubtitle = titleHolder.transform.Find("Subtitle")?.GetComponent<TMP_Text>();
                    
                    Debug.Log($"Title encontrado: {titleTitle != null}, Subtitle encontrado: {titleSubtitle != null}");
                }
                else
                {
                    Debug.LogError("LobbyController: No se encontró TitlePanel en el Canvas!");
                }
                
                // Buscar el objeto Money en el Canvas
                GameObject moneyObject = FindChildByName(canvasTransform, "Money");
                if (moneyObject != null)
                {
                    money = moneyObject.GetComponent<TMP_Text>();
                    Debug.Log($"Money encontrado: {money != null}");
                }
                else
                {
                    Debug.LogError("LobbyController: No se encontró Money en el Canvas!");
                }
            }
            else
            {
                Debug.LogError("LobbyController: No se encontró Canvas en la escena!");
            }
            
            if (titleHolder == null || titleTitle == null || titleSubtitle == null)
            {
                Debug.LogError("LobbyController: No se encontraron las referencias UI!");
            }
        }
    }

    // Método recursivo para buscar un hijo por nombre en toda la jerarquía
    private GameObject FindChildByName(Transform parent, string childName)
    {
        // Buscar en hijos directos primero
        Transform found = parent.Find(childName);
        if (found != null)
            return found.gameObject;
        
        // Si no se encuentra, buscar recursivamente en todos los hijos
        foreach (Transform child in parent)
        {
            GameObject result = FindChildByName(child, childName);
            if (result != null)
                return result;
        }
        
        return null;
    }

    private void Update()
    {
        UpdateMoneyDisplay();
        UpdateRequiredMoney();
        UpdateCurrentRound();
    }

    private void UpdateMoneyDisplay()
    {
        if (money == null) return;
        
        int currentMoney = GameManager.Instance.getCoins();
        
        if (currentMoney.ToString() != money.text)
        {
            money.text = currentMoney.ToString();
        }
    }
    
    private void UpdateRequiredMoney()
    {
        if (requiredMoney == null) return;
        
        int currentMoney = GameManager.Instance.coinsRequired;
        
        if ($"Money to win:\n{currentMoney.ToString()}" != requiredMoney.text)
        {
            requiredMoney.text = $"Money to win:\n{currentMoney.ToString()}";
        }
    }
    
       private void UpdateCurrentRound()
        {
            if (roundStatus == null) return;
            
            int roundsrequired = GameManager.Instance.rounds;
            int currentRound = GameManager.Instance.currentRound;
            
            if ($"Round {currentRound} / {roundsrequired}" != roundStatus.text)
            {
                roundStatus.text = $"Round {currentRound} / {roundsrequired}";
            }
        }
    

    public void DisplayTitle(bool winned)
    {
        // Validar referencias antes de usar
        ValidateReferences();
        
        if (titleHolder == null || titleTitle == null || titleSubtitle == null)
        {
            Debug.LogError("LobbyController: Referencias de título no disponibles!");
            return;
        }
        
        // Detener la animación anterior si existe
        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
            activeAnimationCoroutine = null;
        }
        
        if (winned)
        {
            titleTitle.text = "You Won!";
            titleSubtitle.text = "your money multiplies!";
        }
        else
        {
            titleTitle.text = "You Lose!";
            titleSubtitle.text = "your money divides!";
        }
        
        titleHolder.SetActive(true);
        
        // Guardar la referencia a la nueva coroutine
        activeAnimationCoroutine = StartCoroutine(FadeInOutText(titleTitle, titleSubtitle));
    }
    
    
    public void DisplayTitle(string title, string subtitle)
    {
        // Validar referencias antes de usar
        ValidateReferences();
        
        if (titleHolder == null || titleTitle == null || titleSubtitle == null)
        {
            Debug.LogError("LobbyController: Referencias de título no disponibles!");
            return;
        }
        
        // Detener la animación anterior si existe
        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
            activeAnimationCoroutine = null;
        }
        

            titleTitle.text = title;
            titleSubtitle.text = subtitle;


        
        titleHolder.SetActive(true);
        
        // Guardar la referencia a la nueva coroutine
        activeAnimationCoroutine = StartCoroutine(FadeInOutText(titleTitle, titleSubtitle));
    }

    public void displayGameOver()
    {
        gameoverpanel.SetActive(true);
    }
    

    private IEnumerator FadeInOutText(TMP_Text title, TMP_Text subtitle)
    {
        const float fadeInDuration = 0.5f;
        const float displayDuration = 2f;
        const float fadeOutDuration = 0.5f;
        
        // Inicializar: texto transparente
        SetTextAlpha(title, 0f);
        SetTextAlpha(subtitle, 0f);
        
        // PASO 1: FADE IN
        yield return FadeText(title, subtitle, 0f, 1f, fadeInDuration);
        
        // PASO 2: MANTENER VISIBLE
        yield return new WaitForSeconds(displayDuration);
        
        // PASO 3: FADE OUT
        yield return FadeText(title, subtitle, 1f, 0f, fadeOutDuration);
        
        titleHolder.SetActive(false);
        
        // Limpiar la referencia cuando termina
        activeAnimationCoroutine = null;
    }

    private IEnumerator FadeText(TMP_Text title, TMP_Text subtitle, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            
            SetTextAlpha(title, alpha);
            SetTextAlpha(subtitle, alpha);
            
            yield return null;
        }
        
        SetTextAlpha(title, endAlpha);
        SetTextAlpha(subtitle, endAlpha);
    }

    private void SetTextAlpha(TMP_Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
