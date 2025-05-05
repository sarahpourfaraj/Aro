using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private float pushForce = 3f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.AddForce(new Vector2(moveInput * pushForce, 0));
        }
    }
}