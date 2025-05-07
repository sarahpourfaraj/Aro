using UnityEngine;
using System.Collections;

public class HackedRock : MonoBehaviour
{
    [Header("Hack Settings")]
    [SerializeField] private GameObject hackEffect;
    [SerializeField] private float appearSpeed = 1f;
    
    [Header("Obstacle Control")]
    [SerializeField] private PortalObstacle linkedObstacle;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip activateSound;
    [SerializeField] private float volume = 0.7f;
    
    private bool isActivated = false;
    private bool hasBeenActivated = false; // Track if it's been activated before
    private SpriteRenderer hackRenderer;
    private AudioSource audioSource;

    void Start()
    {
        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f; // 3D sound
        }

        if (hackEffect != null)
        {
            hackRenderer = hackEffect.GetComponent<SpriteRenderer>();
            SetHackAlpha(0f);
            hackEffect.SetActive(true);
        }
        
        if (linkedObstacle == null)
        {
            linkedObstacle = GetComponentInParent<PortalObstacle>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenActivated)
        {
            ActivateHack();
            hasBeenActivated = true; // Mark as activated
        }
    }

    void ActivateHack()
    {
        isActivated = true;
        
        if (linkedObstacle != null)
        {
            linkedObstacle.ActivatePortal();
            PlaySound(activateSound);
        }
        
        if (hackEffect != null)
        {
            if (appearSpeed > 0)
            {
                StartCoroutine(FadeInHack());
            }
            else
            {
                SetHackAlpha(1f);
            }
        }

        if (portalToActivate != null)
        {
            portalToActivate.ActivatePortal();
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator FadeInHack()
    {
        float progress = hackRenderer.color.a;
        while (progress < 1f)
        {
            progress += Time.deltaTime * appearSpeed;
            SetHackAlpha(progress);
            yield return null;
        }
        SetHackAlpha(1f);
    }

    void SetHackAlpha(float alpha)
    {
        if (hackRenderer != null)
        {
            Color c = hackRenderer.color;
            c.a = alpha;
            hackRenderer.color = c;
        }
    }
    
    public PortalObstacle portalToActivate;
}