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
    
    [Header("Player Stats")]
    [SerializeField] private TMP_Text money;
    
    private int cachedMoney = -1;
    [SerializeField] private Canvas mainCanvas;
    private RectTransform titleHolderRect;

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
                
                // Buscar el objeto Money en el Canvas - CORREGIDO
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

    public void DisplayTitle(bool winned)
    {
        // Validar referencias antes de usar
        ValidateReferences();
        
        if (titleHolder == null || titleTitle == null || titleSubtitle == null || titleHolderRect == null)
        {
            Debug.LogError("LobbyController: Referencias de título no disponibles!");
            return;
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
        StartCoroutine(SlideInOutWithFade(titleTitle, titleSubtitle));
    }

    private IEnumerator SlideInOutWithFade(TMP_Text title, TMP_Text subtitle)
    {
        const float slideInDuration = 0.5f;
        const float fadeInDuration = 0.5f;
        const float displayDuration = 2f;
        const float fadeOutDuration = 0.5f;
        const float slideOutDuration = 0.5f;
        
        // Obtener el ancho del canvas para calcular las posiciones
        float canvasWidth = mainCanvas.GetComponent<RectTransform>().rect.width;
        
        // Posiciones: izquierda (fuera), centro, derecha (fuera)
        Vector2 leftPosition = new Vector2(-canvasWidth, titleHolderRect.anchoredPosition.y);
        Vector2 centerPosition = new Vector2(0f, titleHolderRect.anchoredPosition.y);
        Vector2 rightPosition = new Vector2(canvasWidth, titleHolderRect.anchoredPosition.y);
        
        // Inicializar: panel a la izquierda, texto transparente
        titleHolderRect.anchoredPosition = leftPosition;
        SetTextAlpha(title, 0f);
        SetTextAlpha(subtitle, 0f);
        
        // PASO 1: DESLIZAR DE IZQUIERDA AL CENTRO
        yield return SlidePanel(leftPosition, centerPosition, slideInDuration);
        
        // PASO 2: FADE IN DEL TEXTO
        yield return FadeText(title, subtitle, 0f, 1f, fadeInDuration);
        
        // PASO 3: MANTENER VISIBLE
        yield return new WaitForSeconds(displayDuration);
        
        // PASO 4: FADE OUT DEL TEXTO
        yield return FadeText(title, subtitle, 1f, 0f, fadeOutDuration);
        
        // PASO 5: DESLIZAR DEL CENTRO A LA DERECHA
        yield return SlidePanel(centerPosition, rightPosition, slideOutDuration);
        
        titleHolder.SetActive(false);
    }

    private IEnumerator SlidePanel(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease out cubic para movimiento más suave
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            
            titleHolderRect.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            
            yield return null;
        }
        
        titleHolderRect.anchoredPosition = endPos;
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
