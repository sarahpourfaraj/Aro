using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterZone : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private float waterDrag = 5f;
    [SerializeField] private float swimGravityScale = 0.2f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var player = other.GetComponent<Bandit>();
        if (player != null)
        {
            player.EnterWater();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var player = other.GetComponent<Bandit>();
        if (player != null)
        {
            player.ExitWater();
        }
    }
}