using System;
using TMPro;
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

    private TMP_Text currentID;
    private Button buttonLogout;

    void Start()
    {
        try { // messy code, the script is being accessed by different GameObjects, I had to do this
            buttonLogout = GameObject.Find("Button Logout").GetComponent<Button>();

            currentID = GameObject.Find("Note Current").GetComponent<TMP_Text>();
            if (DBC.IS_GUEST_MODE == true)
            {
                currentID.text = "GUEST MODE";
                buttonLogout.gameObject.SetActive(false);
            }
            else
            {
                currentID.text = $"Current ID: {PlayerPrefs.GetString("AccountID")}";
            }

        } catch (NullReferenceException) {}
        
        music = GameObject.Find("Music_Slider").GetComponent<Slider>();
        sfx = GameObject.Find("SFX_Slider").GetComponent<Slider>();
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }
    
    void Update(){}

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


    public void ButtonLogout()
    {
        PlayerPrefs.SetString("AccountID", "");
        PlayerPrefs.SetString("AccountPwd", "");
        PlayerPrefs.SetString("AccountFN", "");
        PlayerPrefs.SetString("AccountMN", "");
        PlayerPrefs.SetString("AccountLN", "");
        PlayerPrefs.SetString("AccountSec", "");
        PlayerPrefs.SetString("PlayerName", "");
        PlayerPrefs.SetString("PlayerGender", "");
        PlayerPrefs.SetInt("isAccountSaved", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Login");

    }


}
