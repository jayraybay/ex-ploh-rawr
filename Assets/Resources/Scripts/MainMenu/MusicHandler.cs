using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    private AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on MusicHandler GameObject.");
        } else {   
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f); 
        }
    }
}
