using System;
using UnityEngine;
using UnityEngine.Events;

public class ClickableSprite : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onClick;

    private Vector3 savedScale;
    
    void OnMouseDown()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.spinWheel();
        
        onClick?.Invoke();
    }
    
    // Opcional: para feedback visual
    void OnMouseEnter()
    {
        savedScale = transform.localScale;
        
        // Cambiar color o escala
        transform.localScale = savedScale * 1.1f;
    }
    
    void OnMouseExit()
    {
        transform.localScale = savedScale;
    }
}