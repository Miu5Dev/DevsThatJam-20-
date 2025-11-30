using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class RouletteScroller : MonoBehaviour
{
    [SerializeField] private RectTransform contentContainer;
    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private RectTransform centerIndicator; // ✅ NECESARIO AHORA
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private AnimationCurve decelerationCurve;
    [SerializeField] private float itemWidth = 150f;
    
    private WeaponSkin guaranteedResult;
    private HorizontalLayoutGroup layoutGroup;
    private List<GameObject> generatedSlots = new List<GameObject>(); // ✅ Guarda los GameObjects
    private List<WeaponSkin> generatedSkins = new List<WeaponSkin>();
    private GraphicRaycaster raycaster; // ✅ Para detectar UI
    private EventSystem eventSystem;

    void Awake()
    {
        layoutGroup = contentContainer.GetComponent<HorizontalLayoutGroup>();
        
        // ✅ Obtiene el raycaster del Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
    }

    public IEnumerator StartRoulette(List<WeaponSkin> pool, WeaponSkin result)
    {
        if (layoutGroup != null)
            layoutGroup.enabled = true;

        yield return ClearItems();

        guaranteedResult = result;
        generatedSkins.Clear();
        generatedSlots.Clear();

        Debug.Log($"lor=cyan>RULETA INICIADA - Resultado garantizado: {result.skinName} ({result.rarity})</color>");

        GenerateItems(pool, 60, 52);

        yield return null;

        contentContainer.anchoredPosition = Vector2.zero;

        float targetPosition = CalculateTargetPosition(52);

        yield return ScrollToPosition(targetPosition, 4f);

        // ✅ Espera un frame para asegurar que todo se renderizó
        yield return null;

        // ✅ RAYCAST PARA DETECTAR QUÉ ITEM ESTÁ BAJO EL INDICADOR
        WeaponSkin detectedSkin = RaycastDetectItem();
        
        if (detectedSkin != null)
        {
            Debug.Log($"lor=lime>✓ RAYCAST DETECTÓ: {detectedSkin.skinName} ({detectedSkin.rarity})</color>");
            guaranteedResult = detectedSkin;
        }
        else
        {
            Debug.LogWarning("Raycast no detectó nada, usando resultado calculado");
        }
    }

    IEnumerator ClearItems()
    {
        foreach (Transform child in contentContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        
        generatedSlots.Clear();
        contentContainer.anchoredPosition = Vector2.zero;
        yield return null;
    }

    void GenerateItems(List<WeaponSkin> pool, int count, int winnerIndex)
    {
        for (int i = 0; i < count; i++)
        {
            WeaponSkin skin = (i == winnerIndex) ? guaranteedResult : GetRandomWeighted(pool);
            generatedSkins.Add(skin);

            if (i == winnerIndex)
            {
                Debug.Log($"lor=green>Índice {i}: GANADOR {guaranteedResult.skinName} ({guaranteedResult.rarity})</color>");
            }

            GameObject slot = Instantiate(itemSlotPrefab, contentContainer);
            slot.name = $"Item_{i}_{skin.skinName}";
            generatedSlots.Add(slot);

            // ✅ Asegura que el slot tenga un Image para raycast
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage == null)
            {
                slotImage = slot.AddComponent<Image>();
                slotImage.color = new Color(1, 1, 1, 0.01f); // Casi invisible pero raycasteable
            }
            slotImage.raycastTarget = true; // ✅ IMPORTANTE

            // ✅ Guarda referencia al skin en el slot
            RouletteSlotData slotData = slot.GetComponent<RouletteSlotData>();
            if (slotData == null)
            {
                slotData = slot.AddComponent<RouletteSlotData>();
            }
            slotData.skin = skin;

            Image bgImage = slot.transform.Find("Background")?.GetComponent<Image>();
            if (bgImage != null)
                bgImage.color = GetColorFromRarity(skin.rarity);

            Image weaponImg = slot.transform.Find("WeaponSprite")?.GetComponent<Image>();
            if (weaponImg != null)
                weaponImg.sprite = skin.skinSprite;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer);
        
        if (layoutGroup != null)
            layoutGroup.enabled = false;
    }

    // ✅ RAYCAST DESDE EL CENTER INDICATOR
    WeaponSkin RaycastDetectItem()
    {
        if (centerIndicator == null)
        {
            Debug.LogError("centerIndicator no está asignado!");
            return guaranteedResult;
        }

        if (raycaster == null)
        {
            Debug.LogError("GraphicRaycaster no encontrado!");
            return guaranteedResult;
        }

        // Posición del centro del indicador en pantalla
        Vector3 indicatorWorldPos = centerIndicator.position;
        
        // Crea datos de raycast
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = RectTransformUtility.WorldToScreenPoint(
            GetComponentInParent<Canvas>().worldCamera, 
            indicatorWorldPos
        );

        // Si el canvas es Overlay, usa directamente la posición
        if (GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
        {
            pointerData.position = indicatorWorldPos;
        }

        // Ejecuta el raycast
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        Debug.Log($"lor=orange>Raycast desde {pointerData.position} encontró {results.Count} objetos</color>");

        // Busca el primer slot de ruleta
        foreach (RaycastResult result in results)
        {
            RouletteSlotData slotData = result.gameObject.GetComponent<RouletteSlotData>();
            if (slotData != null)
            {
                Debug.Log($"lor=lime>Hit en: {result.gameObject.name}</color>");
                return slotData.skin;
            }
            
            // Busca en el padre si el hit fue en un hijo (Background/WeaponSprite)
            slotData = result.gameObject.GetComponentInParent<RouletteSlotData>();
            if (slotData != null)
            {
                Debug.Log($"lor=lime>Hit en hijo de: {slotData.gameObject.name}</color>");
                return slotData.skin;
            }
        }

        Debug.LogWarning("Raycast no encontró ningún RouletteSlotData");
        return guaranteedResult;
    }

    IEnumerator ScrollToPosition(float targetPos, float duration)
    {
        float startPos = contentContainer.anchoredPosition.x;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveValue = decelerationCurve.Evaluate(t);
            
            float newX = Mathf.Lerp(startPos, targetPos, curveValue);
            contentContainer.anchoredPosition = new Vector2(newX, contentContainer.anchoredPosition.y);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        contentContainer.anchoredPosition = new Vector2(targetPos, contentContainer.anchoredPosition.y);
        
        Debug.Log($"lor=yellow>Posición final de ruleta: {targetPos}</color>");
    }

    float CalculateTargetPosition(int itemIndex)
    {
        if (viewportRect == null)
        {
            viewportRect = contentContainer.parent.GetComponent<RectTransform>();
        }

        float viewportCenter = viewportRect.rect.width / 2f;
        float itemCenter = (itemIndex * itemWidth) + (itemWidth / 2f);
        float targetPos = -itemCenter + viewportCenter;
        
        return targetPos;
    }

    public WeaponSkin GetFinalResult()
    {
        return guaranteedResult;
    }

    WeaponSkin GetRandomWeighted(List<WeaponSkin> pool)
    {
        float total = 0f;
        foreach (var skin in pool) total += skin.dropChance;
        
        float random = Random.Range(0, total);
        float current = 0;
        
        foreach (var skin in pool)
        {
            current += skin.dropChance;
            if (random <= current) return skin;
        }
        return pool[0];
    }

    Color GetColorFromRarity(RarityColor rarity)
    {
        return rarity switch
        {
            RarityColor.Common => new Color(0.7f, 0.7f, 0.7f),
            RarityColor.Uncommon => new Color(0.3f, 0.5f, 1f),
            RarityColor.Rare => new Color(0.5f, 0.3f, 1f),
            RarityColor.Epic => new Color(1f, 0.3f, 0.8f),
            RarityColor.Legendary => new Color(1f, 0.8f, 0.2f),
            _ => Color.white
        };
    }
}
