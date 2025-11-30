using System;
using UnityEngine;
using UnityEngine.Events;

public class ClickableSprite : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onClick;

    
    void OnMouseDown()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.spinWheel();
        
        onClick?.Invoke();
    }
    
    // Opcional: para feedback visual
    void OnMouseEnter()
    {
        // Cambiar color o escala
        transform.localScale = Vector3.one * 1.1f;
    }
    
    void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }
}