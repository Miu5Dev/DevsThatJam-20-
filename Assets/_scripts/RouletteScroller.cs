using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RouletteScroller : MonoBehaviour
{
    [SerializeField] private RectTransform contentContainer; // El objeto con HorizontalLayoutGroup
    [SerializeField] private GameObject itemSlotPrefab; // Prefab simple: Image con tamaño fijo
    [SerializeField] private AnimationCurve decelerationCurve; // Crea curva en Inspector
    [SerializeField] private float itemWidth = 150f; // Ancho de cada item
    
    private WeaponSkin guaranteedResult;

    public IEnumerator StartRoulette(List<WeaponSkin> pool, WeaponSkin result)
    {
        ClearItems();
        guaranteedResult = result;

        GenerateItems(pool, 60, 52);

        // IMPORTANTE: resetear posición antes de animar
        contentContainer.anchoredPosition = Vector2.zero;

        float targetPosition = CalculateTargetPosition(52);

        yield return ScrollToPosition(targetPosition, 4f);
    }

    void ClearItems()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateItems(List<WeaponSkin> pool, int count, int winnerIndex)
    {
        for (int i = 0; i < count; i++)
        {
            WeaponSkin skin = (i == winnerIndex) ? guaranteedResult : GetRandomWeighted(pool);

            GameObject slot = Instantiate(itemSlotPrefab, contentContainer);

            // Fondo según rareza
            Image bgImage = slot.transform.Find("Background")?.GetComponent<Image>();
            if (bgImage != null)
                bgImage.color = GetColorFromRarity(skin.rarity);

            // Sprite del arma
            Image weaponImg = slot.transform.Find("WeaponSprite")?.GetComponent<Image>();
            if (weaponImg != null)
                weaponImg.sprite = skin.skinSprite;
        }
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
    }

    float CalculateTargetPosition(int itemIndex)
    {
        // Calcula para centrar el item en la pantalla
        float screenCenter = Screen.width / 2f;
        return -(itemIndex * itemWidth) + screenCenter - (itemWidth / 2f);
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
