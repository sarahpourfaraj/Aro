using UnityEngine;
using UnityEngine.SceneManagement;

public class HiddenCollectible : MonoBehaviour
{
    public enum DiamondType { Red, Yellow, Blue, Green }
    public DiamondType diamondType;

    [Header("Floating Settings")]
    [SerializeField] private float floatHeight = 0.1f;
    [SerializeField] private float floatSpeed = 3.5f;
    [SerializeField] private bool floats = true;

    [Header("Red Diamond Settings")]
    public float speedMultiplier = 1.5f;
    public float boostDuration = 3f;

    [Header("Yellow Diamond Settings")]
    public GameObject backgroundToRemove;
    public AudioClip yellowSceneMusic; // Add this for yellow diamond music change

    [Header("Green Diamond Settings")]
    public GameObject linkedObstacle;
    public GameObject backgroundToRemoveGreen;
    public AudioClip greenSceneMusic; // Renamed from newSceneMusic for clarity

    [Header("Blue Diamond Settings")]
    public WaterZone[] waterZonesToActivate;

    [Header("Sound Effects")]
    public AudioClip redDiamondSound;
    public AudioClip yellowDiamondSound;
    public AudioClip blueDiamondSound;
    public AudioClip greenDiamondSound;

    private Vector3 startPosition;
    private bool isCollected = false;
    private Collider2D diamondCollider;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        diamondCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        floats = (diamondType == DiamondType.Red || diamondType == DiamondType.Blue);

        if (diamondType == DiamondType.Green && linkedObstacle != null)
        {
            diamondCollider.enabled = !linkedObstacle.activeInHierarchy;
        }
    }

    void Update()
    {
        if (floats && !isCollected)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        if (diamondType == DiamondType.Green && !isCollected && linkedObstacle != null)
        {
            diamondCollider.enabled = !linkedObstacle.activeInHierarchy;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isCollected) return;

        var player = other.GetComponent<Bandit>();
        if (player == null) return;

        isCollected = true;

        switch (diamondType)
        {
            case DiamondType.Red:
                if (redDiamondSound != null)
                    audioSource.PlayOneShot(redDiamondSound);
                player.BoostSpeed(speedMultiplier, boostDuration);
                break;

            case DiamondType.Yellow:
                if (yellowDiamondSound != null)
                    audioSource.PlayOneShot(yellowDiamondSound);
                if (backgroundToRemove != null)
                    Destroy(backgroundToRemove);
                
                // Change scene music for yellow diamond
                if (yellowSceneMusic != null)
                {
                    ChangeSceneMusic(yellowSceneMusic);
                }
                break;

            case DiamondType.Green:
                if (greenDiamondSound != null)
                    audioSource.PlayOneShot(greenDiamondSound);
                if (backgroundToRemoveGreen != null)
                    Destroy(backgroundToRemoveGreen);
                
                // Change scene music for green diamond
                if (greenSceneMusic != null)
                {
                    ChangeSceneMusic(greenSceneMusic);
                }
                break;

            case DiamondType.Blue:
                if (blueDiamondSound != null)
                    audioSource.PlayOneShot(blueDiamondSound);
                player.EnableSwimming();
                foreach (var waterZone in waterZonesToActivate)
                {
                    if (waterZone != null)
                    {
                        waterZone.ActivateWater();
                    }
                }
                break;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }

    private void ChangeSceneMusic(AudioClip newMusic)
    {
        SceneMusic sceneMusic = FindObjectOfType<SceneMusic>();
        if (sceneMusic != null)
        {
            sceneMusic.ChangeMusic(newMusic);
        }
    }
}