using UnityEngine;
using System.Collections;


public class Bandit : MonoBehaviour
{

    [Header("Swimming")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float swimJumpForce = 5f;
    [SerializeField] private bool canSwim = false;
    private bool isInWater = false;
    private float originalJumpForce;

    private WaterZone currentWaterZone;
    private float originalDrag;
    private float originalAngularDrag;
    private float originalGravityScale;



    [Header("Movement")]
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    [Header("Combat")]
    [SerializeField] private int maxEnemyHits = 2;
    [SerializeField] private float attackRange = 1.5f;
    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private GameObject attackHitbox; // Assign in Inspector

    //fall
    [Header("Respawn")]
    [SerializeField] private float deathYLevel = -10f; // Y position where player dies
    private Vector3 spawnPosition;

    private int currentHealth;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;
    private Vector3 m_spawnPosition;
    private bool m_isRespawning = false;
    private int enemyHits = 0;

    private float originalSpeed;


    void Start()
    {
        originalJumpForce = m_jumpForce;
        spawnPosition = transform.position;
        originalSpeed = m_speed;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
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
        float inputY = isInWater ? Input.GetAxis("Vertical") : 0;

        if (inputX > 0) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        m_body2d.velocity = new Vector2(inputX * m_speed, isInWater ? inputY * m_speed : m_body2d.velocity.y);
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
        if (Input.GetKeyDown("e")) ToggleDeathState();
        if (Input.GetKeyDown("q")) m_animator.SetTrigger("Hurt");
        if (Input.GetMouseButtonDown(0)) CheckAttack();
        if (Input.GetKeyDown("f")) m_combatIdle = !m_combatIdle;

        // Only allow jumping if grounded or in water
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

    void CheckAttack()
    {
        m_animator.SetTrigger("Attack");

        // Enable hitbox temporarily
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            StartCoroutine(DisableHitbox(0.2f)); // Disable after 0.2 seconds
        }
    }
    IEnumerator DisableHitbox(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackHitbox.SetActive(false);
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
        currentHealth = maxHealth; // Reset health on respawn
    }
    public class CollisionDebugger : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log($"{name} collided with {col.gameObject.name}");
        }
        void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log($"{name} triggered by {col.gameObject.name}");
        }
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

    //for red stone
    public void BoostSpeed(float multiplier, float duration)
    {
        m_speed = originalSpeed * multiplier;
        Invoke(nameof(ResetSpeed), duration);
    }

    private void ResetSpeed()
    {
        m_speed = originalSpeed;
    }
    public void EnableSwimming()
    {
        canSwim = true;
    }

    public bool CanSwim()
    {
        return canSwim;
    }

    public void SetInWater(bool inWater)
    {
        isInWater = inWater;
        m_animator.SetBool("InWater", inWater);

        if (inWater)
        {
            // Adjust physics for water
            m_body2d.gravityScale = 0.5f;
            m_speed = swimSpeed;
            m_jumpForce = swimJumpForce;
        }
        else
        {
            // Reset to normal physics
            m_body2d.gravityScale = 1f;
            m_speed = originalSpeed;
            m_jumpForce = originalJumpForce;
        }
    }
    public void EnterWater(WaterZone waterZone)
    {
        if (!canSwim) return;

        isInWater = true;
        currentWaterZone = waterZone;

        // Save original physics values
        originalDrag = m_body2d.drag;
        originalAngularDrag = m_body2d.angularDrag;
        originalGravityScale = m_body2d.gravityScale;

        // Apply water physics
        m_body2d.drag = waterZone.GetWaterDrag();
        m_body2d.angularDrag = waterZone.GetWaterAngularDrag();
        m_body2d.gravityScale = waterZone.GetSwimGravityScale();

        m_animator.SetBool("InWater", true);
    }

    public void ExitWater()
    {
        if (!isInWater) return;

        isInWater = false;
        currentWaterZone = null;

        // Restore original physics
        m_body2d.drag = originalDrag;
        m_body2d.angularDrag = originalAngularDrag;
        m_body2d.gravityScale = originalGravityScale;

        m_animator.SetBool("InWater", false);
    }

}
