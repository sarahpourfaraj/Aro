using UnityEngine;

public class ColorStone : MonoBehaviour
{
    // Auto-finds the filter if not assigned
    private WorldColorFilter worldFilter;
    
    void Start()
    {
        if (worldFilter == null)
            worldFilter = FindObjectOfType<WorldColorFilter>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && worldFilter != null)
        {
            worldFilter.RemoveColorFilter();
            Destroy(gameObject);
        }
    }
}