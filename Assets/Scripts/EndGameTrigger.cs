using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip victoryMusic;
    public float musicVolume = 1f;
    
    [Header("Effects")]
    public ParticleSystem confettiEffect;
    public GameObject victoryUI; 
    
    private bool _hasTriggered = false;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            _hasTriggered = true;
            
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.isKinematic = true;  // Freeze in place
            }

            if (victoryMusic != null)
            {
                _audioSource.clip = victoryMusic;
                _audioSource.volume = musicVolume;
                _audioSource.Play();
            }

            if (confettiEffect != null)
            {
                confettiEffect.Play();
            }

            if (victoryUI != null)
            {
                victoryUI.SetActive(true);
            }

            Time.timeScale = 0f; 
        }
    }
}