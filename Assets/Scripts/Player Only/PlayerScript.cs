using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public static GameObject PLAYER;

    private GameObject target;
    private NPC NPCscript;
    private bool hasTriggered = false;
    
    private Button buttonInteract;
    private TMP_InputField inputField;

    // <!Static>
    public static Button UI_INTERACT;
    public static TMP_InputField UI_DIALOGUE;

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

        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        float z = PlayerPrefs.GetFloat("PlayerZ");
        if (y < 0) { x = 0; y = 0; z = 0; }
        transform.position = new Vector3(x, y, z);


        buttonInteract = GameObject.FindGameObjectWithTag("UI_Interact").GetComponent<Button>();
        inputField = GameObject.FindGameObjectWithTag("UI_Dialogue").GetComponent<TMP_InputField>();

        PlayerInteract.UI_INTERACT = buttonInteract;
        PlayerInteract.UI_DIALOGUE = inputField;


        GameObject.FindGameObjectWithTag("Canvas_Terminal").GetComponent<Canvas>().enabled = false;
        GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>().enabled = true;
    }

    void Update()
    {
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

        if (other.gameObject.CompareTag("NPC") && target == null)
        {
            target = other.gameObject;
            //Debug.Log(target.name);
            NPCscript = target.GetComponent<NPC>();
            buttonInteract.onClick.AddListener(NPCscript.Interact);
            buttonInteract.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
        buttonInteract.interactable = false;

        buttonInteract.onClick.RemoveListener(NPCscript.Interact);
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
