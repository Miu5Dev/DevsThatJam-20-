using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuletaPhysics : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isSpinning = false;
    private bool isSnapping = false;
    
    [Header("Spin Settings")]
    [SerializeField] private float minSpinForce = 1500f;
    [SerializeField] private float maxSpinForce = 4500f;
    [SerializeField] private float stopThreshold = 10f;
    
    [Header("Physics Settings")]
    [SerializeField] private float angularDrag = 1.5f;
    [SerializeField] private float finalDragMultiplier = 0.92f;
    [SerializeField] private float randomDragVariation = 0.3f;
    
    [Header("Snapping")]
    [SerializeField] private int numberOfOptions = 8;
    [SerializeField] private float snapDuration = 0.8f;
    [SerializeField] private AnimationCurve snapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float snapRandomOffset = 0.15f;
    
    [Header("Corner Colliders")]
    [SerializeField] private List<GameObject> cornerColliders = new List<GameObject>();
    [SerializeField] private bool autoSetupColliders = true;
    [SerializeField] private float colliderRadius = 0.1f;
    
    [Header("Indicator/Pointer")]
    [SerializeField] private GameObject indicator;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private float tickVolume = 0.5f;
    
    private float anglePerOption;
    private float targetSnapAngle;
    private int selectedOption = -1;
    private float lastTickTime = 0f;
    private float minTimeBetweenTicks = 0.05f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Configurar Rigidbody2D
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = 0;
        rb.angularDamping = angularDrag;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        anglePerOption = 360f / numberOfOptions;
        
        // Configurar audio
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Auto-setup de colliders si está habilitado
        if (autoSetupColliders)
        {
            SetupCornerColliders();
        }
        
        // Buscar indicador automáticamente si no está asignado
        if (indicator == null)
        {
            indicator = GameObject.Find("Indicator");
            if (indicator == null)
            {
                indicator = GameObject.FindGameObjectWithTag("Indicator");
            }
        }
    }
    
    void SetupCornerColliders()
    {
        // Configurar todos los corner colliders encontrados
        foreach (GameObject corner in cornerColliders)
        {
            if (corner == null) continue;
            
            // Agregar Rigidbody2D estático para colisiones físicas
            Rigidbody2D cornerRb = corner.GetComponent<Rigidbody2D>();
            if (cornerRb == null)
            {
                cornerRb = corner.AddComponent<Rigidbody2D>();
            }
            cornerRb.bodyType = RigidbodyType2D.Kinematic;
            cornerRb.simulated = true;
            
            // Setup collider
            Collider2D col = corner.GetComponent<Collider2D>();
            if (col == null)
            {
                CircleCollider2D circleCol = corner.AddComponent<CircleCollider2D>();
                circleCol.radius = colliderRadius;
                col = circleCol;
            }
            
            // NO es trigger - colisión física real
            col.isTrigger = false;
            
            // Asegurar que tenga el tag correcto
            if (!corner.CompareTag("CornerCollider"))
            {
                corner.tag = "CornerCollider";
            }
        }
        
        Debug.Log($"Configurados {cornerColliders.Count} corner colliders con físicas");
    }
    
    void Update()
    {
        // Presiona Space para girar
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinning)
        {
            SpinWheel();
        }
    }
    
    void FixedUpdate()
    {
        if (isSpinning && !isSnapping)
        {
            float currentSpeed = Mathf.Abs(rb.angularVelocity);
            
            // Cuando la velocidad es baja, aplicar fricción extra
            if (currentSpeed < stopThreshold && currentSpeed > 0.5f)
            {
                rb.angularVelocity *= finalDragMultiplier;
            }
            
            // Cuando casi se detiene, hacer snap
            if (currentSpeed < 1f)
            {
                isSnapping = true;
                StartCoroutine(SnapToNearestOption());
            }
        }
    }
    
    public void SpinWheel()
    {
        if (isSpinning) return;
        
        isSpinning = true;
        isSnapping = false;
        selectedOption = -1;
        
        // Aplicar torque aleatorio con rango amplio
        float torque = Random.Range(minSpinForce, maxSpinForce);
        rb.AddTorque(torque, ForceMode2D.Impulse);
        
        // Variar el drag para cada spin
        float dragVariation = Random.Range(1f - randomDragVariation, 1f + randomDragVariation);
        rb.angularDamping = angularDrag * dragVariation;
        
        // Reproducir sonido de giro
        if (audioSource != null && spinSound != null)
        {
            audioSource.PlayOneShot(spinSound);
        }
        
        Debug.Log($"Ruleta girando... Torque: {torque:F1}, Drag: {rb.angularDamping:F2}");
    }
    
    IEnumerator SnapToNearestOption()
    {
        yield return new WaitForFixedUpdate();
        
        // Calcular ángulo actual SIN normalizar primero
        float rawAngle = rb.rotation;
        
        // Encontrar la opción más cercana usando el ángulo crudo
        float nearestOptionIndex = Mathf.Round(rawAngle / anglePerOption);
        
        // Aplicar el offset aleatorio a la opción, no al ángulo
        float offsetSteps = Random.Range(-snapRandomOffset, snapRandomOffset);
        nearestOptionIndex += offsetSteps;
        
        // Calcular el ángulo objetivo
        targetSnapAngle = nearestOptionIndex * anglePerOption;
        
        // Normalizar solo para calcular qué opción es (para display)
        float normalizedTargetAngle = targetSnapAngle % 360f;
        if (normalizedTargetAngle < 0) normalizedTargetAngle += 360f;
        
        selectedOption = (Mathf.RoundToInt(normalizedTargetAngle / anglePerOption) % numberOfOptions) + 1;
        if (selectedOption <= 0) selectedOption += numberOfOptions;
        
        Debug.Log($"Ángulo crudo: {rawAngle:F2}°, Target: {targetSnapAngle:F2}°, Opción: {selectedOption}");
        
        // Animación suave con curva
        float startAngle = rb.rotation;
        float elapsed = 0f;
        
        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = snapCurve.Evaluate(elapsed / snapDuration);
            float newAngle = Mathf.LerpAngle(startAngle, targetSnapAngle, t);
            rb.MoveRotation(newAngle);
            
            yield return null;
        }
        
        // Asegurar que termina exactamente en el ángulo objetivo
        rb.MoveRotation(targetSnapAngle);
        rb.angularVelocity = 0;
        isSnapping = false;
        isSpinning = false;
        OnSpinComplete();
    }
    
    void OnSpinComplete()
    {
        // Restaurar el drag original para el próximo giro
        rb.angularDamping = angularDrag;
        
        Debug.Log($"¡Ruleta detenida en opción {selectedOption}!");
        
        // Aquí puedes llamar eventos
        // GameManager.Instance.OnRouletteResult(selectedOption);
    }
    
    // Este método es llamado por el PointerPhysics cuando colisiona
    public void OnPointerHit()
    {
        if (!isSpinning || isSnapping) return;
        
        // Evitar ticks muy seguidos
        if (Time.time - lastTickTime < minTimeBetweenTicks)
            return;
        
        lastTickTime = Time.time;
        
        // Reproducir tick
        if (audioSource != null && tickSound != null)
        {
            audioSource.PlayOneShot(tickSound, tickVolume);
        }
    }
    
    // Métodos públicos
    public int GetSelectedOption()
    {
        return selectedOption;
    }
    
    public bool IsSpinning()
    {
        return isSpinning;
    }
    
    public void SetNumberOfOptions(int count)
    {
        numberOfOptions = count;
        anglePerOption = 360f / numberOfOptions;
    }
}
