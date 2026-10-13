using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioSource audioSource;
    void Awake()
    {
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on SoundHandler GameObject.");
        } else {   
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        }
    }

    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float lastValue = audioSource.volume;
        if (audioSource.volume != lastValue)
        {
            Debug.Log("audioSource volume changed to " + audioSource.volume);
        }
    }
}
