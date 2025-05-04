using System.Collections;
using UnityEngine;

public class DamageableChest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int jumpsRequired = 6;
    [SerializeField] private int attacksRequired = 2;
    [SerializeField] private Color hurtColor = Color.white;
    [SerializeField] private float flashDuration = 0.2f;

    [Header("References")]
    [SerializeField] private Animator chestAnimator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private int jumpCount = 0;
    private int attackCount = 0;
    private bool isOpen = false;

    void Awake()
    {
        // Automatically get the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on chest!", this);
        }

        originalColor = spriteRenderer.color;
    }

    void Start()
    {
        originalColor = spriteRenderer.color;
    }

    // Called when player jumps on the chest
    public void TakeJumpDamage()
    {
        if (isOpen) return;

        jumpCount++;
        FlashColor();

        if (jumpCount >= jumpsRequired)
        {
            OpenChest();
        }
    }

    // Called when player attacks the chest
    public void TakeAttackDamage()
    {
        if (isOpen) return;

        attackCount++;
        FlashColor();

        if (attackCount >= attacksRequired)
        {
            OpenChest();
        }
    }

    void FlashColor()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void OpenChest()
    {
        isOpen = true;
        chestAnimator.SetTrigger("Open");
        // Add your reward spawning logic here
    }
}