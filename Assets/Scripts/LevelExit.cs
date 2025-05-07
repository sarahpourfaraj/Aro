using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLevelExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            SceneManager.LoadScene("Gold City"); // Replace with your scene name
        }
    }
}