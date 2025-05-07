using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private GameObject spikesObstacle;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public Transform[] waypoints;
    public float reachThreshold = 0.1f;

    private int currentHealth;
    private int currentWaypointIndex = 0;
    private float lastDamageTime;
    
    private Vector2[] outwardDirections = {
        Vector2.down, Vector2.left, Vector2.up, Vector2.right
    };

    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = -damageCooldown;

        // Make sure spikes are active at start
        if (spikesObstacle != null)
        {
            spikesObstacle.SetActive(true);
        }
    }

    void Update()
    {
        PatrolBetweenWaypoints();
    }

    public void TakeDamage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Disable spikes when enemy dies
        if (spikesObstacle != null)
        {
            spikesObstacle.SetActive(false);
            
            // Also disable the DeathObstacle component if it exists
            DeathObstacle deathObstacle = spikesObstacle.GetComponent<DeathObstacle>();
            if (deathObstacle != null)
            {
                deathObstacle.enabled = false;
            }
            
            // Disable collider if it exists
            Collider2D spikeCollider = spikesObstacle.GetComponent<Collider2D>();
            if (spikeCollider != null)
            {
                spikeCollider.enabled = false;
            }
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
        if (collision.gameObject.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;
            HurtPlayer(collision.gameObject);
        }
    }

    private void HurtPlayer(GameObject player)
    {
        Bandit playerScript = player.GetComponent<Bandit>();
        if (playerScript != null)
        {
            playerScript.m_animator.SetTrigger("Hurt");
            ApplyKnockback(player);
        }
    }

    private void ApplyKnockback(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
            playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
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