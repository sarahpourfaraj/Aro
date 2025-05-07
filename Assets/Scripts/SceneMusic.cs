using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusic : MonoBehaviour
{
    [System.Serializable]
    public class LevelMusic
    {
        public string sceneName;
        public AudioClip musicTrack;
    }

    public LevelMusic[] levelMusicList;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (LevelMusic lm in levelMusicList)
        {
            if (scene.name == lm.sceneName)
            {
                audioSource.clip = lm.musicTrack;
                audioSource.Play();
                break;
            }
        }
    }

    // Add this new method to change music dynamically
    public void ChangeMusic(AudioClip newMusic)
    {
        if (newMusic == null || audioSource == null) return;
        
        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();
        
        // Update the current scene's music in the list
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (var levelMusic in levelMusicList)
        {
            if (levelMusic.sceneName == currentScene)
            {
                levelMusic.musicTrack = newMusic;
                break;
            }
        }
    }
}