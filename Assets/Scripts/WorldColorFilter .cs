using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using System.Collections;


public class WorldColorFilter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 3f;
    
    [Header("References")]
    [SerializeField] private Image uiFilter; // For UI method
    [SerializeField] private SpriteRenderer spriteFilter; // For Sprite method

    private float initialAlpha;

    void Start()
    {
        // Store starting opacity
        if (uiFilter != null) initialAlpha = uiFilter.color.a;
        if (spriteFilter != null) initialAlpha = spriteFilter.color.a;
    }

    public void RemoveColorFilter()
    {
        StartCoroutine(FadeOutFilter());
    }

    IEnumerator FadeOutFilter()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(initialAlpha, 0f, elapsed/fadeDuration);
            
            // Update both possible filter types
            if (uiFilter != null)
            {
                Color c = uiFilter.color;
                c.a = newAlpha;
                uiFilter.color = c;
            }
            
            if (spriteFilter != null)
            {
                Color c = spriteFilter.color;
                c.a = newAlpha;
                spriteFilter.color = c;
            }
            
            yield return null;
        }
        
        // Optional: Disable when fully transparent
        gameObject.SetActive(false);
    }
}