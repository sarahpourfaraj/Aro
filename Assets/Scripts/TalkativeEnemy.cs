using UnityEngine;
using TMPro;
using System.Collections;

public class TalkativeEnemy : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [Tooltip("The conversation lines to display")]
    [SerializeField] private string[] dialogueLines;
    [Tooltip("How close the player needs to be")]
    [SerializeField] private float dialogueRange = 3f;
    [Tooltip("Time each line stays visible")]
    [SerializeField] private float timePerLine = 3f;
    [Tooltip("Characters per second for typewriter effect")]
    [SerializeField] private float charsPerSecond = 30f;
    [Tooltip("Player layer for detection")]
    [SerializeField] private LayerMask playerLayer;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject exclamationMark;

    private Transform player;
    private int currentLine = 0;
    private float dialogueTimer;
    private bool isTalking = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        InitializeDialogue();
        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        CheckPlayerDistance();
    }

    void InitializeDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (exclamationMark != null) exclamationMark.SetActive(false);
    }

    void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, 10f, playerLayer);
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            player = playerCollider.transform;
        }
    }

    void CheckPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= dialogueRange)
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            UpdateDialogue();
        }
        else if (isTalking)
        {
            EndDialogue();
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;
        
        if (exclamationMark != null) 
            exclamationMark.SetActive(false);
        
        if (dialoguePanel != null) 
            dialoguePanel.SetActive(true);
        
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (typingCoroutine != null) 
            StopCoroutine(typingCoroutine);
        
        typingCoroutine = StartCoroutine(TypeText(dialogueLines[currentLine]));
        dialogueTimer = timePerLine;
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
        
        isTyping = false;
    }

    void UpdateDialogue()
    {
        dialogueTimer -= Time.deltaTime;

        if (dialogueTimer <= 0 && !isTyping)
        {
            currentLine = (currentLine + 1) % dialogueLines.Length;
            ShowCurrentLine();
        }
    }

    void EndDialogue()
    {
        isTalking = false;
        
        if (typingCoroutine != null) 
            StopCoroutine(typingCoroutine);
        
        if (dialoguePanel != null) 
            dialoguePanel.SetActive(false);
        
        if (exclamationMark != null) 
            exclamationMark.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dialogueRange);
    }
}