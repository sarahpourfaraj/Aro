using UnityEngine;
public class DeathObstacle : MonoBehaviour 
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // First check if it's the player
        if (other.CompareTag("Player")) 
        {
            // Then safely get the Bandit component
            Bandit player = other.GetComponent<Bandit>();
            if (player != null) 
            {
                player.DieAndRespawn();
            }
            else
            {
                Debug.LogError("Player has no Bandit script!", other.gameObject);
            }
        }
    }
}