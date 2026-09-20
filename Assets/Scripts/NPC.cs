// Define the interface
using System;
using System.Collections;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Rendering.MaterialUpgrader;

public class NPC : MonoBehaviour
{

    // Inspector fields
    public bool isEntity; // when interacted, this NPC face the player
    public bool canSkip; // can skip dialogue

    public SpriteRenderer npcSprite;
    public AnimatorController animatorController;
    public Animator animator; // unused in Inspector
    private string currentMsg; // when currently in dialogue

    public void Start()
    {
        /* Depends on derived classes
        npcSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();*/

    }

    public void Update()
    {
        // Look at the camera, depends on derived class
        transform.rotation = Quaternion.LookRotation(PlayerInteract.PLAYER_CAMERA.transform.forward);

        if (currentMsg != null && Input.GetMouseButton(0) && canSkip) {
            StopAllCoroutines();
            PlayerInteract.UI_DIALOGUE.text = currentMsg;
            PlayerInteract.UI_INTERACT.enabled = true;
            currentMsg = null;
        }
    }

    public virtual void Interact() {}
    public virtual void Speak(string msg) {
        var dialogue = GameObject.Find("Dialogue Text");
        var content = dialogue.GetComponent<TMP_InputField>();
        //content.text = msg;

        var image = dialogue.GetComponent<Image>();
        image.enabled = true;
        content.enabled = true;

        currentMsg = msg;
        //StartCoroutine(TypeSentence(msg, 0.01f));
        StartCoroutine(TypeSentence(msg, 0.01f));
    }

    IEnumerator TypeSentence(string sentence, float cps)
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
        PlayerInteract.UI_INTERACT.gameObject.SetActive(true);
        PlayerInteract.UI_JOYSTICK.gameObject.SetActive(true);
    }

}
