using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    [Header("Combat")]
    [SerializeField] private int maxEnemyHits = 2;
    [SerializeField] private float attackRange = 1.5f;
    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private GameObject attackHitbox; // Assign in Inspector

    [Header("Respawn")]
    [SerializeField] private float deathYLevel = -10f; // Y position where player dies
    private Vector3 spawnPosition;

    [Header("Swimming")]
    [SerializeField] private bool canSwim = false;
    private bool isInWater = false;
    private int currentHealth;

    public Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;
    private Vector3 m_spawnPosition;
    private bool m_isRespawning = false;
    private int enemyHits = 0;

    private float originalSpeed;
    private float originalGravity;
    void Start()
    {
        spawnPosition = transform.position;
        originalSpeed = m_speed;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        originalGravity = m_body2d.gravityScale;
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        m_spawnPosition = transform.position;
        currentHealth = maxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chest landing check
        if (m_grounded && collision.gameObject.CompareTag("Chest"))
        {
            if (m_body2d.velocity.y <= 0)
            {
                collision.gameObject.GetComponent<DamageableChest>().TakeJumpDamage();
            }
        }

        // Enemy contact damage
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(); // Use the proper damage method

            // Apply knockback
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            m_body2d.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
        }
    }

    void Update()
    {
        CheckFallDeath();
        HandleGroundDetection();
        HandleMovement();
        HandleAnimations();
        HandleInput();

        // Simple water floating effect
        if (isInWater && !m_grounded)
        {
            // Apply gentle up and down movement
            m_body2d.AddForce(new Vector2(0, Mathf.Sin(Time.time * 2f) * 0.5f));
        }
    }

    public void CheckFallDeath()
    {
        if (transform.position.y < deathYLevel)
        {
            Respawn();
        }
    }

    void HandleGroundDetection()
    {
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);
    }

    void HandleAnimations()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);
        else
            m_animator.SetInteger("AnimState", 0);
    }

    void HandleInput()
    {
        // if (Input.GetKeyDown("e")) ToggleDeathState();
        // if (Input.GetKeyDown("q")) m_animator.SetTrigger("Hurt");
        // if (Input.GetMouseButtonDown(0)) CheckAttack();
        // if (Input.GetKeyDown("f")) m_combatIdle = !m_combatIdle;

        if (Input.GetKeyDown("space") && (m_grounded || isInWater))
        {
            Jump();
        }
    }

    void ToggleDeathState()
    {
        if (!m_isDead) m_animator.SetTrigger("Death");
        else m_animator.SetTrigger("Recover");
        m_isDead = !m_isDead;
    }

    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

    public void DieAndRespawn()
    {
        if (m_isRespawning) return;

        m_isRespawning = true;
        m_animator.SetTrigger("Death");
        m_isDead = true;
        m_body2d.velocity = Vector2.zero;
        m_body2d.simulated = false;
        Invoke("Respawn", 1f);
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        m_body2d.velocity = Vector2.zero;
        transform.position = m_spawnPosition;
        m_body2d.simulated = true;
        m_animator.SetTrigger("Recover");
        m_isDead = false;
        m_isRespawning = false;
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        currentHealth--;
        m_animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            DieAndRespawn();
        }
    }

    public void BoostSpeed(float multiplier, float duration)
    {
        m_speed = originalSpeed * multiplier;
        Invoke(nameof(ResetSpeed), duration);
    }

    private void ResetSpeed()
    {
        m_speed = originalSpeed;
    }

    public void EnterWater()
    {
        if (!canSwim)
        {
            DieAndRespawn();
            return;
        }

        isInWater = true;
        m_body2d.gravityScale = originalGravity * 0.2f;
        m_body2d.drag = 5f;
    }

    public void ExitWater()
    {
        isInWater = false;
        m_body2d.gravityScale = originalGravity;
        m_body2d.drag = 0f;
    }
    public void EnableSwimming()
    {
        canSwim = true;
    }
}