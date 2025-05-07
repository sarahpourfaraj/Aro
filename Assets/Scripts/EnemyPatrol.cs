using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] private float waitTime = 1f;

    [Header("Player Death")]
    [SerializeField] private float killRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    private Transform currentTarget;
    private bool isWaiting = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = pointB;
        UpdateFacingDirection();
    }

    void Update()
    {
        if (isWaiting) return;
        
        // Movement
        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        // Check if reached target
        if (Vector2.Distance(transform.position, currentTarget.position) < reachThreshold)
        {
            StartCoroutine(WaitAndSwitch());
        }

        // Check for player contact
        CheckPlayerContact();
    }

    void CheckPlayerContact()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, killRange, playerLayer);
        if (player != null && player.CompareTag("Player"))
        {
            Bandit playerScript = player.GetComponent<Bandit>();
            if (playerScript != null)
            {
                playerScript.DieAndRespawn();
            }
        }
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        UpdateFacingDirection();
        isWaiting = false;
    }

    void UpdateFacingDirection()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (currentTarget == pointA);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw patrol path
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }

        // Draw kill range
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, killRange);
    }
}