using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class settings_Script : MonoBehaviour
{
    private Slider music;
    private Slider sfx;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music = GameObject.Find("Music_Slider").GetComponent<Slider>();
        sfx = GameObject.Find("SFX_Slider").GetComponent<Slider>();
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickBack()
    {
        PlayerPrefs.Save();
        Debug.Log("Clicked Back");
    }

    public void updateSFXVolume(float volume)
    {
        sfxSource = GameObject.Find("SoundHandler").GetComponent<AudioSource>();
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log("SFX Slider set to: " + volume);
        sfxSource.Play();
    }

    public void updateMusicVolume(float volume)
    {
        musicSource = GameObject.Find("MusicHandler").GetComponent<AudioSource>();
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        Debug.Log("Music Slider set to: " + volume);
    }
}
