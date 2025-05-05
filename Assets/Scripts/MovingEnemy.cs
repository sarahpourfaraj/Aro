using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public int startingPoint = 0;
    public Transform[] points;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug = true;

    private int _currentWaypointIndex;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (!ValidateWaypoints())
        {
            Debug.LogError("Waypoint validation failed!", this);
            enabled = false;
            return;
        }

        _currentWaypointIndex = startingPoint;
        transform.position = points[_currentWaypointIndex].position;
        
        if (_showDebug) LogWaypointInfo();
    }

    bool ValidateWaypoints()
    {
        // Check if array exists and has elements
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("Waypoints array is empty or null", this);
            return false;
        }

        // Check each waypoint in the array
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogWarning($"Waypoint {i} is null!", this);
                return false;
            }

            if (points[i].position == transform.position)
            {
                Debug.LogWarning($"Waypoint {i} has same position as enemy!", this);
            }
        }

        return true;
    }

    void Update()
    {
        MoveBetweenWaypoints();
    }

    void MoveBetweenWaypoints()
    {
        Transform target = points[_currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.02f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % points.Length;
            if (_showDebug) Debug.Log($"Reached waypoint, moving to index {_currentWaypointIndex} ({points[_currentWaypointIndex].name})");
        }
    }

    void LogWaypointInfo()
    {
        Debug.Log($"Enemy has {points.Length} waypoints assigned:");
        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log($"Waypoint {i}: {(points[i] != null ? points[i].name : "NULL")} " +
                     $"at position {points[i].position}");
        }
        Debug.Log($"Starting at waypoint index {startingPoint} ({points[startingPoint].name})");
    }

    void OnDrawGizmosSelected()
    {
        if (points == null || points.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawSphere(points[i].position, 0.1f);
            
            int nextIndex = (i + 1) % points.Length;
            if (points[nextIndex] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(points[i].position, points[nextIndex].position);
                Gizmos.color = Color.red;
            }
        }
    }
}