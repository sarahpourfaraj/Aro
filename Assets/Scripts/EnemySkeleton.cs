using UnityEngine;
using System.Collections;

public class EnemySkeleton : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 2; // Dies after 2 hits now
    [SerializeField] private float whiteFlashDuration = 3f; // Longer white flash
    [SerializeField] private float redFlashDuration = 6f; // Longer red flash
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private Color deathColor = Color.red;
    [SerializeField] private GameObject spikesObstacle;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public Transform[] waypoints;
    public float reachThreshold = 0.1f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private int currentWaypointIndex = 0;
    
    private Vector2[] outwardDirections = {
        Vector2.down,    // Moving right
        Vector2.left,    // Moving down
        Vector2.up,      // Moving left
        Vector2.right    // Moving up
    };

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        PatrolBetweenWaypoints();
    }

    public void TakeDamage()
    {
        currentHealth--;
        
        // Always flash white when hit
        StopAllCoroutines();
        StartCoroutine(FlashEffect(hitColor, whiteFlashDuration));

        if (currentHealth <= 0)
        {
            // Flash red right before death
            StartCoroutine(DeathEffect());
        }
    }

    private IEnumerator FlashEffect(Color flashColor, float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator DeathEffect()
    {
        // First flash white (final hit)
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(whiteFlashDuration);
        spriteRenderer.color = originalColor;
        
        // Then flash red before dying
        spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(redFlashDuration);
        
        if (spikesObstacle != null)
        {
            spikesObstacle.SetActive(false);
        }
        Destroy(gameObject);
    }

    void PatrolBetweenWaypoints()
    {
        if (waypoints == null || waypoints.Length != 4) return;

        Transform currentWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

        Vector2 moveDir = (currentWaypoint.position - transform.position).normalized;
        
        if (moveDir.magnitude > 0.01f)
        {
            int sideIndex = currentWaypointIndex;
            Vector2 outwardDir = outwardDirections[sideIndex];
            float angle = Mathf.Atan2(outwardDir.y, outwardDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Vector2.Distance(transform.position, currentWaypoint.position) < reachThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % 4;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Bandit player = collision.gameObject.GetComponent<Bandit>();
            if (player != null)
            {
                player.TakeDamage();
                Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
                player.GetComponent<Rigidbody2D>().AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage();
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length == 4)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < 4; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    Gizmos.DrawLine(waypoints[i].position, waypoints[(i + 1) % 4].position);
                    
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(waypoints[i].position, outwardDirections[i] * 0.5f);
                    Gizmos.color = Color.cyan;
                }
            }
        }
    }
}