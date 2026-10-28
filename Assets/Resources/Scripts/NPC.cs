// Define the interface
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Rendering.MaterialUpgrader;

public class NPC : MonoBehaviour
{

    // Inspector fields
    public bool isEntity; // when interacted, this NPC face the player
    public bool canSkip; // can skip dialogue

    // Stupid solution for towers and 3D models
    public bool isModel;

    public SpriteRenderer npcSprite;
    public RuntimeAnimatorController animatorController;
    public Animator animator; // unused in Inspector
    private string currentMsg; // when currently in dialogue; null means not in dialogue

    public virtual void Start()
    {
        /* Depends on derived classes
        npcSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();*/

        /* Generic
        npcSprite = GetComponent<Renderer>();
        animator = GetComponent<Animator>();*/
    }

    public virtual void Update()
    {
        // Look at the camera, depends on derived class
        if (!isModel) {
            try {
                transform.rotation = Quaternion.LookRotation(PlayerInteract.PLAYER_CAMERA.transform.forward);
            } catch (NullReferenceException) { // for some reason, in Minigame
            }
        
        }


        if (currentMsg != null && Input.GetMouseButton(0) && canSkip) {
            StopAllCoroutines();
            PlayerInteract.UI_DIALOGUE.text = currentMsg;
            PlayerInteract.UI_INTERACT.enabled = true;
            currentMsg = null;

            PlayerInteract.UI_JOYSTICK.gameObject.SetActive(true);
            PlayerInteract.UI_INTERACT.gameObject.SetActive(true);
        }
    }

    public virtual void Interact() {
        /*
        Transform p = PlayerInteract.PLAYER.transform.Find("Player Sprite");
        SpriteRenderer s = p.GetComponent<SpriteRenderer>();
        Debug.Log(p + " " + s);
             if (p.position.x > transform.position.x) { s.flipX = false; }
        else if (p.position.x < transform.position.x) { s.flipX = true;  }*/
    }

    public virtual void Speak(string msg) {
        Speak(msg, false);
    }
    public virtual void Speak(string msg, bool isExplained) {
        var dialogue = GameObject.Find("Dialogue Text");
        var content = dialogue.GetComponent<TMP_InputField>();
        //content.text = msg;

        var image = dialogue.GetComponent<Image>();
        image.enabled = true;
        content.enabled = true;

        currentMsg = msg;
        //StartCoroutine(TypeSentence(msg, 0.01f));
        StartCoroutine(TypeSentence(msg, 0.01f, isExplained));
    }

    IEnumerator TypeSentence(string sentence, float cps, bool isExplained)
    {
        var dialogue = GameObject.Find("Dialogue Text");
        var content = dialogue.GetComponent<TMP_InputField>();
        PlayerInteract.UI_INTERACT.gameObject.SetActive(false);
        PlayerInteract.UI_JOYSTICK.gameObject.SetActive(false);


        content.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (isEntity) {
                if      (transform.position.x > PlayerInteract.PLAYER.transform.position.x) npcSprite.flipX = true;
                else if (transform.position.x < PlayerInteract.PLAYER.transform.position.x) npcSprite.flipX = false;
            }
            
            content.text += letter;
            yield return new WaitForSeconds(cps);
        }

        if (!isExplained)
        {
            PlayerInteract.UI_INTERACT.gameObject.SetActive(true);
            PlayerInteract.UI_JOYSTICK.gameObject.SetActive(true);

            PlayerInteract.UI_DIALOGUE_PREV.gameObject.SetActive(false);
            PlayerInteract.UI_DIALOGUE_NEXT.gameObject.SetActive(false);
        }


    }

}
