using UnityEngine;
public class HiddenCollectible : MonoBehaviour
{
    public enum DiamondType { Red, Yellow, Blue }
    public DiamondType diamondType;

    [Header("Red Diamond Settings")]
    public float speedMultiplier = 1.5f;
    public float boostDuration = 3f;

    [Header("Yellow Diamond Settings")]
    public GameObject backgroundToRemove;

    [Header("Blue Diamond Settings")]
    public WaterZone[] waterZonesToActivate;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<Bandit>();
        if (player == null) return;

        // Make sure we have a Collider2D component
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        switch (diamondType)
        {
            case DiamondType.Red:
                player.BoostSpeed(speedMultiplier, boostDuration);
                break;

            case DiamondType.Yellow:
                if (backgroundToRemove != null)
                    Destroy(backgroundToRemove);
                break;

            case DiamondType.Blue:
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

        // Play collection effect (optional)
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Destroy after a delay to allow any effects to play
        Destroy(gameObject, 0.5f);
    }
}