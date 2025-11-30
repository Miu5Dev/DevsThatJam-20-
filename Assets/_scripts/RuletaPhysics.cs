using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class RouletteResultEvent : UnityEvent<int> { }

public class RuletaPhysics : MonoBehaviour
{
   [SerializeField] private Rigidbody2D rb;
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
    
    [Header("Corner Colliders (solo para ticks de sonido si quieres)")]
    [SerializeField] private List<GameObject> cornerColliders = new List<GameObject>();
    [SerializeField] private bool autoSetupColliders = true;
    [SerializeField] private float colliderRadius = 0.1f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private float tickVolume = 0.5f;

    [Header("Events")]
    public RouletteResultEvent onSpinComplete = new RouletteResultEvent();
    public UnityEvent onSpinStart = new UnityEvent();
    
    [Header("options")]
    public List<RouletteOption> options = new List<RouletteOption>();
    
    
    private float anglePerOption;
    private float targetSnapAngle;
    private int selectedOption = -1;
    private float lastTickTime = 0f;
    private float minTimeBetweenTicks = 0.05f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if(GameManager.Instance != null)
            GameManager.Instance.currentRoulette = this;
        
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0;
        rb.angularDamping = angularDrag;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        anglePerOption = 360f / numberOfOptions;
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (autoSetupColliders)
            SetupCornerColliders();
    }
    
    void SetupCornerColliders()
    {
        foreach (GameObject corner in cornerColliders)
        {
            if (corner == null) continue;
            
            Rigidbody2D cornerRb = corner.GetComponent<Rigidbody2D>();
            if (cornerRb == null) cornerRb = corner.AddComponent<Rigidbody2D>();
            cornerRb.bodyType = RigidbodyType2D.Kinematic;
            cornerRb.simulated = true;
            
            Collider2D col = corner.GetComponent<Collider2D>();
            if (col == null)
            {
                CircleCollider2D circleCol = corner.AddComponent<CircleCollider2D>();
                circleCol.radius = colliderRadius;
                col = circleCol;
            }
            col.isTrigger = true; // solo para ticks, no física real
        }
    }
    
    void FixedUpdate()
    {
        if (isSpinning && !isSnapping)
        {
            float currentSpeed = Mathf.Abs(rb.angularVelocity);
            
            if (currentSpeed < stopThreshold && currentSpeed > 0.5f)
                rb.angularVelocity *= finalDragMultiplier;
            
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
        
        onSpinStart?.Invoke();
        
        float torque = Random.Range(minSpinForce, maxSpinForce);
        rb.AddTorque(torque, ForceMode2D.Impulse);
        
        float dragVariation = Random.Range(1f - randomDragVariation, 1f + randomDragVariation);
        rb.angularDamping = angularDrag * dragVariation;
        
        if (audioSource != null && spinSound != null)
            audioSource.PlayOneShot(spinSound);
    }
    
    IEnumerator SnapToNearestOption()
    {
        yield return new WaitForFixedUpdate();
        
        float rawAngle = rb.rotation;
        float nearestOptionIndex = Mathf.Round(rawAngle / anglePerOption);
        float offsetSteps = Random.Range(-snapRandomOffset, snapRandomOffset);
        nearestOptionIndex += offsetSteps;
        
        targetSnapAngle = nearestOptionIndex * anglePerOption;
        
        float normalizedTargetAngle = targetSnapAngle % 360f;
        if (normalizedTargetAngle < 0) normalizedTargetAngle += 360f;
        
        selectedOption = (Mathf.RoundToInt(normalizedTargetAngle / anglePerOption) % numberOfOptions);
        if (selectedOption <= 0) selectedOption += numberOfOptions;
        
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
        
        rb.MoveRotation(targetSnapAngle);
        rb.angularVelocity = 0;
        isSnapping = false;
        isSpinning = false;
        OnSpinComplete();
    }
    
    void OnSpinComplete()
    {
        rb.angularDamping = angularDrag;
        onSpinComplete?.Invoke(selectedOption);
    }
    
    // API pública
    public int GetSelectedOption() => selectedOption;
    public bool IsSpinning() => isSpinning;
    public void SetNumberOfOptions(int count)
    {
        numberOfOptions = count;
        anglePerOption = 360f / numberOfOptions;
    }
}
