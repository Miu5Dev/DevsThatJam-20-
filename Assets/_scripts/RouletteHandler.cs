using System.Collections;
using UnityEngine;

public class RouletteHandler : MonoBehaviour
{
    [SerializeField] private Transform wheelPart;
    [SerializeField] private Transform indicator;
    [SerializeField] private float spinPower = 3000f;
    [SerializeField] private float stopPower = 0.5f;
    [SerializeField] private int numberOfOptions = 8;
    [SerializeField] private float raycastDistance = 10f; // Distancia del raycast

    private float currentSpeed = 0f;
    private bool isSpinning = false;

    private void Start()
    {
        SpinWheel();
    }

    public void SpinWheel()
    {
        if (!isSpinning)
        {
            StartCoroutine(Spin());
        }
    }
    
    private IEnumerator Spin()
    {
        isSpinning = true;
        currentSpeed = spinPower;
        
        // Gira mientras tenga velocidad
        while (currentSpeed > 0.1f)
        {
            currentSpeed -= currentSpeed * stopPower * Time.deltaTime;
            wheelPart.Rotate(0f, 0f, currentSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Snap al ángulo más cercano
        float finalAngle = SnapToNearestOption();
        yield return StartCoroutine(SmoothSnapToAngle(finalAngle));
        
        isSpinning = false;
        
        // Detecta el resultado con raycast
        int result = DetectResultWithRaycast();
        if (result != -1)
        {
            Debug.Log($"Resultado: Opción {result}");
        }
        else
        {
            Debug.LogWarning("No se detectó ninguna opción con el raycast");
        }
    }
    
    private float SnapToNearestOption()
    {
        float anglePerOption = 360f / numberOfOptions;
        float currentAngle = wheelPart.eulerAngles.z;
        
        // Encuentra el múltiplo más cercano
        float nearestOption = Mathf.Round(currentAngle / anglePerOption);
        float targetAngle = nearestOption * anglePerOption;
        
        return targetAngle;
    }

    private IEnumerator SmoothSnapToAngle(float targetAngle)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Quaternion startRotation = wheelPart.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
    
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            wheelPart.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
    
        wheelPart.rotation = targetRotation;
    }
    
    private int DetectResultWithRaycast()
    {
        // Lanza el raycast desde el indicador hacia abajo
        Vector3 rayOrigin = indicator.position;
        Vector3 rayDirection = -indicator.up; // Apunta hacia abajo del indicador
        
        // Para 2D usa Physics2D.Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance);
        
        // Para debugging visual
        Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red, 2f);
        
        if (hit.collider != null)
        {
            RouletteOption option = hit.collider.GetComponent<RouletteOption>();
            if (option != null)
            {
                return option.getMultiplier();
            }
        }
        
        return -1; // No se encontró nada
    }
    
    // Para debugging en el editor
    private void OnDrawGizmos()
    {
        if (indicator != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rayOrigin = indicator.position;
            Vector3 rayDirection = -indicator.up;
            Gizmos.DrawRay(rayOrigin, rayDirection * raycastDistance);
        }
    }
}
