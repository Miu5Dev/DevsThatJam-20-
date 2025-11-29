using UnityEngine;
using UnityEngine.Events;

public class ClickableSprite : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onClick;
    
    void OnMouseDown()
    {
        Debug.Log($"Clicked on {gameObject.name}");
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