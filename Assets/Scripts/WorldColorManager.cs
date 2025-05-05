using UnityEngine;

public class WorldColorManager : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject; // Drag your BG here
    
    public void RevealColors()
    {
        if (backgroundObject != null)
        {
            Destroy(backgroundObject); // Completely removes the background
            // Alternative: backgroundObject.SetActive(false); // Just hides it
        }
    }
}