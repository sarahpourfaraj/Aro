using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterZone : MonoBehaviour
{
    private bool isActive = false;
    [SerializeField] private AudioClip waterMusic; // Assign this in the inspector
    private AudioSource waterAudioSource;

    private void Start()
    {
        // Set up the audio source
        waterAudioSource = gameObject.AddComponent<AudioSource>();
        waterAudioSource.clip = waterMusic;
        waterAudioSource.loop = true;
        waterAudioSource.playOnAwake = false;
    }

    public void ActivateWater()
    {
        isActive = true;
        // Optional: Change water appearance here
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var player = other.GetComponent<Bandit>();
        if (player != null)
        {
            player.EnterWater();
            
            // Play water music if assigned
            if (waterMusic != null && !waterAudioSource.isPlaying)
            {
                waterAudioSource.Play();
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var player = other.GetComponent<Bandit>();
        if (player != null)
        {
            player.ExitWater();
            
            // Stop water music if playing
            if (waterAudioSource.isPlaying)
            {
                waterAudioSource.Stop();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive && collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<Bandit>();
            if (player != null)
            {
                player.DieAndRespawn();
            }
        }
    }
}