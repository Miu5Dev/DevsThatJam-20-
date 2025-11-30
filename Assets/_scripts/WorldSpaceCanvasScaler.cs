using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldSpaceCanvasScaler : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float distanceFromCamera = 10f;
    [SerializeField] private bool updateEveryFrame = false;
    
    private Canvas canvas;
    private RectTransform rectTransform;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        
        if (targetCamera == null)
            targetCamera = Camera.main;
        
        canvas.renderMode = RenderMode.WorldSpace;
        
        ScaleToCamera();
    }

    void Update()
    {
        if (updateEveryFrame)
        {
            ScaleToCamera();
        }
    }

    void ScaleToCamera()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("No hay cámara asignada");
            return;
        }

        // Posiciona el canvas frente a la cámara
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
        transform.rotation = targetCamera.transform.rotation;

        if (targetCamera.orthographic)
        {
            // ✅ Para cámaras ortográficas
            float cameraHeight = targetCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * targetCamera.aspect;

            // Setea el tamaño del RectTransform directamente
            rectTransform.sizeDelta = new Vector2(cameraWidth, cameraHeight);
            
            // ✅ Escala a 1:1 (cada unidad del canvas = 1 unidad del mundo)
            transform.localScale = Vector3.one;
        }
        else
        {
            // ✅ Para cámaras en perspectiva
            float height = 2f * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distanceFromCamera;
            float width = height * targetCamera.aspect;

            rectTransform.sizeDelta = new Vector2(width, height);
            transform.localScale = Vector3.one;
        }
    }

    public void RefreshScale()
    {
        ScaleToCamera();
    }
}
