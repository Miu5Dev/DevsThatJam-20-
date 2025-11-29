using UnityEngine;

public class PointerPhysics : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float pointerMass = 0.5f;
    [SerializeField] private float pointerDrag = 0.5f;
    [SerializeField] private float pointerAngularDrag = 2f;
    [SerializeField] private float springStrength = 50f;
    [SerializeField] private float springDamping = 5f;
    
    [Header("Collision Settings")]
    [SerializeField] private float collisionForceMultiplier = 1f;
    [SerializeField] private PhysicsMaterial2D bounceMaterial;
    
    [Header("References")]
    [SerializeField] private RuletaPhysics ruletaPhysics;
    
    private Rigidbody2D rb;
    private HingeJoint2D hingeJoint;
    private Collider2D pointerCollider;
    private float restAngle = 0f;
    
    void Start()
    {
        SetupPhysics();
        
        // Buscar referencia a la ruleta si no está asignada
        if (ruletaPhysics == null)
        {
            ruletaPhysics = FindObjectOfType<RuletaPhysics>();
        }
    }
    
    void SetupPhysics()
    {
        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.mass = pointerMass;
        rb.linearDamping = pointerDrag;
        rb.angularDamping = pointerAngularDrag;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        // Setup Collider
        pointerCollider = GetComponent<Collider2D>();
        if (pointerCollider == null)
        {
            // Crear un BoxCollider2D por defecto
            BoxCollider2D boxCol = gameObject.AddComponent<BoxCollider2D>();
            boxCol.size = new Vector2(0.2f, 1f);
            boxCol.offset = new Vector2(0, 0.5f);
            pointerCollider = boxCol;
        }
        
        // Aplicar material de física si está asignado
        if (bounceMaterial != null)
        {
            pointerCollider.sharedMaterial = bounceMaterial;
        }
        else
        {
            // Crear material con bounce
            PhysicsMaterial2D defaultMaterial = new PhysicsMaterial2D("PointerBounce");
            defaultMaterial.bounciness = 0.3f;
            defaultMaterial.friction = 0.1f;
            pointerCollider.sharedMaterial = defaultMaterial;
        }
        
        // Setup HingeJoint2D
        hingeJoint = GetComponent<HingeJoint2D>();
        if (hingeJoint == null)
        {
            hingeJoint = gameObject.AddComponent<HingeJoint2D>();
        }
        
        hingeJoint.autoConfigureConnectedAnchor = false;
        hingeJoint.anchor = Vector2.zero;
        hingeJoint.connectedAnchor = transform.position;
        hingeJoint.enableCollision = false;
        hingeJoint.useLimits = false;
        
        // Configurar motor del spring
        JointMotor2D motor = hingeJoint.motor;
        motor.motorSpeed = 0;
        motor.maxMotorTorque = springStrength;
        hingeJoint.motor = motor;
        hingeJoint.useMotor = true;
        
        // Guardar ángulo de reposo
        restAngle = transform.localEulerAngles.z;
    }
    
    void FixedUpdate()
    {
        // Aplicar "spring" para volver a la posición original
        ApplySpringForce();
    }
    
    void ApplySpringForce()
    {
        // Calcular diferencia de ángulo con la posición de reposo
        float currentAngle = rb.rotation;
        float angleDifference = Mathf.DeltaAngle(currentAngle, restAngle);
        
        // Aplicar torque para volver a la posición (spring effect)
        float springTorque = angleDifference * springStrength;
        float dampingTorque = -rb.angularVelocity * springDamping;
        
        rb.AddTorque(springTorque + dampingTorque);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si colisionó con un corner collider
        if (collision.gameObject.CompareTag("CornerCollider"))
        {
            // Notificar a la ruleta que hubo un hit
            if (ruletaPhysics != null)
            {
                ruletaPhysics.OnPointerHit();
            }
            
            // Aplicar fuerza adicional si es necesario
            if (collisionForceMultiplier > 1f && collision.contacts.Length > 0)
            {
                Vector2 normal = collision.contacts[0].normal;
                rb.AddForceAtPosition(-normal * collisionForceMultiplier, collision.contacts[0].point, ForceMode2D.Impulse);
            }
        }
    }
    
    // Métodos públicos para ajustar en runtime
    public void SetSpringStrength(float strength)
    {
        springStrength = strength;
    }
    
    public void SetSpringDamping(float damping)
    {
        springDamping = damping;
    }
}
