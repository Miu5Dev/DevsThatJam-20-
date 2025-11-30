using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CaseBox : MonoBehaviour, IPointerClickHandler
{
    public event Action<CaseBox> OnCaseClicked;
    
    [SerializeField] private Image boxImage; // Referencia al sprite
    [SerializeField] private Sprite closedSprite; // Sprite de caja cerrada
    [SerializeField] private Sprite openedSprite; // Sprite de caja abierta
    
    private bool canClick = true;
    private bool isOpened = false;

    private void Start()
    {
        // Asegura que empiece con sprite cerrado
        if (boxImage != null && closedSprite != null)
        {
            boxImage.sprite = closedSprite;
            boxImage.color = Color.white; // Color normal
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick || isOpened) return;
        
        OnCaseClicked?.Invoke(this);
        MarkAsOpened();
    }
    
    private void MarkAsOpened()
    {
        isOpened = true;
        canClick = false;
        
        if (boxImage != null)
        {
            if (openedSprite != null)
                boxImage.sprite = openedSprite;
            
            // Tinte gris
            boxImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
    
    public void EnableClick(bool enable) 
    {
        canClick = enable;
    }
    
    public void ResetBox()
    {
        isOpened = false;
        canClick = true;
        
        if (boxImage != null)
        {
            if (closedSprite != null)
                boxImage.sprite = closedSprite;
            
            // Restaurar color normal
            boxImage.color = Color.white;
        }
    }
}