using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public static GameObject PLAYER;
    public static string GENDER;


    private GameObject target;
    private NPC NPCscript;
    private bool hasTriggered = false;
    
    private Button buttonInteract;
    private TMP_InputField inputField;

    // <!Static>
    public static Button UI_INTERACT;
    public static TMP_InputField UI_DIALOGUE;
    public static Image UI_JOYSTICK;
    public static Camera PLAYER_CAMERA;
    // </Static>


    // <Movement>
    public float SPEED = 5.0f;
    public bool isMirrored;
    Vector3 moveVector;
    public CharacterController characterController;
    public MobileController mobileController;
    public Animator animator;


    // </Movement>


    void Start()
    {
        PlayerInteract.PLAYER = gameObject;
        GENDER = PlayerPrefs.GetString("PlayerGender");
        animator = transform.GetChild(1).GetComponent<Animator>();
       

             if (GENDER == "M") animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerMale_Controller");      //Debug.Log("MALE!!");
        else if (GENDER == "F") animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerFemale_Controller");    //Debug.Log("FEMALE!!");
        else                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerNonbinary_Controller"); //Debug.Log("NONBINARY!!");


        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        float z = PlayerPrefs.GetFloat("PlayerZ");
        if (y < 0) { x = 0; y = 0; z = 0; }
        transform.position = new Vector3(x, y, z);


        buttonInteract = GameObject.FindGameObjectWithTag("UI_Interact").GetComponent<Button>();
        inputField = GameObject.FindGameObjectWithTag("UI_Dialogue").GetComponent<TMP_InputField>();

        PlayerInteract.UI_INTERACT = buttonInteract;
        PlayerInteract.UI_DIALOGUE = inputField;
        PlayerInteract.UI_JOYSTICK = GameObject.FindGameObjectWithTag("UI_Joystick").GetComponent<Image>();
        PlayerInteract.PLAYER_CAMERA = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //GameObject.FindGameObjectWithTag("Canvas_Terminal").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Terminal Multiple").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Terminal Identify").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Terminal Trivia").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Terminal Ready").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Terminal Results").GetComponent<Canvas>().enabled = false;

        // ...
        GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>().enabled = true;
    }

    void Update()
    {
        transform.Find("Player Sprite").GetComponent<SpriteRenderer>().transform.rotation = Quaternion.LookRotation(PLAYER_CAMERA.transform.forward);
        CharacterMove();
        //Debug.Log(target);
    }

    private void CharacterMove()
    {
        moveVector = Vector3.zero;
        moveVector.x = mobileController.Horizontal() * SPEED;
        moveVector.z = mobileController.Vertical() * SPEED;

        //characterController.Move(moveVector * Time.deltaTime); // without gravity
        characterController.SimpleMove(moveVector); // with gravity

        // Save location
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", transform.position.z);
        PlayerPrefs.Save();

        SpriteRenderer spriteRenderer = transform.Find("Player Sprite").GetComponent<SpriteRenderer>();
        if (moveVector.x > 0)      { this.isMirrored = false;}
        else if (moveVector.x < 0) { this.isMirrored = true; }
        spriteRenderer.flipX = this.isMirrored;

        if (moveVector.x != 0 || moveVector.y != 0) animator.SetBool("isMoving", true);
        else                                        animator.SetBool("isMoving", false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        hasTriggered = true;
        //Debug.Log(other);

        if (other.gameObject.CompareTag("NPC") && target == null)
        {
            target = other.gameObject;
            //Debug.Log(target.name);
            NPCscript = target.GetComponent<NPC>();
            if (target.GetComponent<NPC>() != null) NPCscript = target.GetComponent<NPC>();
            buttonInteract.onClick.AddListener(NPCscript.Interact);
            buttonInteract.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
        buttonInteract.interactable = false;

        if (NPCscript != null) buttonInteract.onClick.RemoveListener(NPCscript.Interact);
        NPCscript = null;
        hasTriggered = false;

        var dialogue = GameObject.Find("Dialogue Text");
        var content = dialogue.GetComponent<TMP_InputField>();
        content.text = "";
        var image = dialogue.GetComponent<Image>();
        image.enabled = false;
        content.enabled = false;
    }


    public void ForceSpeak(string msg) {
        PlayerInteract.UI_DIALOGUE.text = msg;
    }

    public void ToggleInteraction(bool here) {
        PlayerInteract.UI_INTERACT.gameObject.SetActive(here);
    }

    public void TogglePlayerUI(bool here) {
        
    }


}
