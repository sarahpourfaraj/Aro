using UnityEngine;

public class GraveyardScroll : MonoBehaviour 
{
    [Tooltip("How fast this layer moves (0 = static, 1 = moves with camera)")]
    [Range(0, 1)]
    public float scrollSpeed = 0.5f;
    
    private Transform cameraTransform;
    private Vector3 startPos;
    private bool isReady = false;

    void Awake()
    {
        // Get camera reference early
        FindMainCamera();
    }

    void Start()
    {
        InitializePosition();
    }

    void FindMainCamera()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No camera tagged 'MainCamera' in scene!");
            return;
        }
        
        cameraTransform = Camera.main.transform;
        Debug.Log($"Camera assigned to {gameObject.name}: {cameraTransform.name}", this);
    }

    void InitializePosition()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera transform not assigned!", this);
            return;
        }

        startPos = transform.position;
        isReady = true;
    }

    void LateUpdate()
    {
        if (!isReady) return;
        
        float newX = startPos.x + (cameraTransform.position.x * scrollSpeed);
        transform.position = new Vector3(newX, startPos.y, startPos.z);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
        }
    }
}