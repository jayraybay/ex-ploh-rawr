using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplainerInteract : NPC
{
    [SerializeField]
    public List<String> dialogue = new List<String>();

    private int index;

    new void Start()
    {
        //npcSprite = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();

        SpriteRenderer explainerSprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        explainerSprite.GetComponent<Animator>().runtimeAnimatorController = animatorController;
    }
    new void Update() {
        transform.rotation = Quaternion.LookRotation(PlayerInteract.PLAYER_CAMERA.transform.forward);
    }

    public override void Interact()
    {
        base.Interact();

        // First-Line
        index = 0;
        Speak(dialogue[index], true);
        //Debug.Log(index);

        PlayerInteract.UI_INTERACT.gameObject.SetActive(false);
        PlayerInteract.UI_DIALOGUE_PREV.gameObject.SetActive(false);
        PlayerInteract.UI_DIALOGUE_NEXT.gameObject.SetActive(true);
    }


    public void SayNext() {
        if (index >= dialogue.Count - 1) { return; }

        index++;
        Speak(dialogue[index], true);

        if (index >= dialogue.Count - 1) // explaining done!
        {
            PlayerInteract.UI_DIALOGUE_NEXT.gameObject.SetActive(false);
            PlayerInteract.UI_DIALOGUE_PREV.gameObject.SetActive(true);
            PlayerInteract.UI_INTERACT.gameObject.SetActive(true);
            PlayerInteract.UI_JOYSTICK.gameObject.SetActive(true);
        }
        if (index != 0) {
            PlayerInteract.UI_DIALOGUE_PREV.gameObject.SetActive(true);
        }

        //Debug.Log(index);
    }

    public void SayPrev()
    {
        if (index == 0) { return; }

        index--;
        Speak(dialogue[index], true);

        //Debug.Log(index);

        if (index == 0)
        {
            PlayerInteract.UI_DIALOGUE_PREV.gameObject.SetActive(false);
        }
        if (index <= dialogue.Count - 1) {
            PlayerInteract.UI_DIALOGUE_NEXT.gameObject.SetActive(true);
        }
    }
}
