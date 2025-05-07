using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PortalObstacle : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName; // Name of the scene to load
    [SerializeField] private float transitionDelay = 0.5f; // Delay before loading new scene
    
    private Collider2D[] allColliders;
    private bool isActivated = false;
    private bool isTransitioning = false;

    void Start()
    {
        allColliders = GetComponentsInChildren<Collider2D>();
        InitializeState();
    }

    void InitializeState()
    {
        SetAllCollidersTrigger(false);
    }

    public void ActivatePortal()
    {
        if (isActivated) return;
        
        isActivated = true;
        SetAllCollidersTrigger(true);
    }

    public void DeactivatePortal()
    {
        if (!isActivated) return;
        
        isActivated = false;
        SetAllCollidersTrigger(false);
    }

    private void SetAllCollidersTrigger(bool isTrigger)
    {
        foreach (Collider2D collider in allColliders)
        {
            if (collider != null)
            {
                collider.isTrigger = isTrigger;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActivated && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Bandit>()?.DieAndRespawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && !isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(TransitionScenes());
        }
    }

    IEnumerator TransitionScenes()
    {
        isTransitioning = true;
        
        // Optional: Fade out effect
        // yield return StartCoroutine(FadeOut());
        
        yield return new WaitForSeconds(transitionDelay);
        
        // Load the new scene, replacing the current one
        SceneManager.LoadScene(nextSceneName);
        
        isTransitioning = false;
    }
}