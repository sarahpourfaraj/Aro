using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;

    private int i;

    void Start()
    {
        transform.position = points[startingPoint].position;
        i = startingPoint;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i = (i + 1) % points.Length; // Loop waypoints smoothly
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Store the original parent (if not already stored)
            if (collision.transform.parent != null)
            {
                // Optional: You can cache this in a variable if needed
                // But we'll just re-parent on exit
            }
            collision.transform.SetParent(transform); // Stick to platform
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Find the "Player" parent GameObject in the scene
            GameObject playerParent = GameObject.Find("Player"); // Adjust name if needed
            if (playerParent != null)
            {
                collision.transform.SetParent(playerParent.transform);
            }
            else
            {
                Debug.LogWarning("Player parent object not found in scene!");
            }
        }
    }
}