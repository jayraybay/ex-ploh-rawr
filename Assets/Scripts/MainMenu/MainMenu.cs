using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    private GameObject SoundHandler;
    private AudioSource audioSource;
    private Canvas settings;
    private Canvas main;
    private SpriteRenderer genderSprite;
    private SpriteRenderer shadowblob;

    void Start()
    {
        string gender = PlayerPrefs.GetString("PlayerGender");
        Animator animator = transform.GetChild(8).GetComponent<Animator>();

        if      (gender == "M") animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerMale_Controller");
        else if (gender == "F") animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerFemale_Controller");
        else                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerNonbinary_Controller");
    }
    
    void Update()
    {
        
    }

    void Awake()
    {
        //sound
        SoundHandler = GameObject.Find("SoundHandler");
        audioSource = SoundHandler.GetComponent<AudioSource>();

        //ui
        settings = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        main = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        genderSprite = GameObject.Find("Gender Sprite").GetComponent<SpriteRenderer>();
        shadowblob = GameObject.Find("Shadow Blob").GetComponent<SpriteRenderer>();
    }

    
    public void ClickPlay() {
        Debug.Log("Clicked Play");
        audioSource.Play();
        StartCoroutine(DramaticWait());
    }
    private IEnumerator DramaticWait() //coroutine
    {
        Debug.Log("Waiting for 5 seconds before loading the scene...");
        audioSource.clip = Resources.Load<AudioClip>("Sound/eyecatch");
        audioSource.Play();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("InGame");
    }

    public void ClickProgress() {
        Debug.Log("Clicked Progress");
        audioSource.Play();
    }

    public void ClickQuit()
    {
        Debug.Log("Clicked Quit");
        audioSource.Play();
        Application.Quit();
        
    }

    public void ClickSettings() {

 
        settings.enabled = true;
        initiateMainMenu(false);

        audioSource.Play();
        Debug.Log("Clicked Settings");
    }

    public void ClickBack() {
        audioSource.Play();
        settings.enabled = false;
        initiateMainMenu(true);

        Debug.Log("Returned to main menu");
    }
    public void ClickFlashcards()
    {
        Debug.Log("Clicked Flashcards");
        audioSource.Play();
    }


    public void ClickPlayer()
    {
        SceneManager.LoadScene("Player");
        audioSource.Play();
        Debug.Log("Edit Player");

    }

    public void playButtonAudio()
    {
        audioSource.Play();
    }

    public void initiateMainMenu(bool a)
    {
        main.enabled = a;
        genderSprite.enabled = a;
        shadowblob.enabled = a;
    }
}
