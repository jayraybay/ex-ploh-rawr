using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NPC_Tower : NPC
{
    GameObject confirmationUI;
    int index = 0;
    string[] dialogue = {
        "You are heading to a tower defense minigame.",
        "If you ever failed, you can try again.",
    };
    
    string npcName = "Tower"; // Helps Quest system identify which NPC was interacted with
    Confirmation confirmationScript;

    new void Start(){
        // does not exists, maybe you deleted it?
        //confirmationUI = GameObject.Find("ConfirmationUI");
        //confirmationScript = confirmationUI.GetComponent<Confirmation>();
    }
 
    new void Update() {
        base.Update();
    }

    // This function is called when a player interacted this NPC
    public override void Interact() // Changed it to protected override for Quest system to work properly
    {
        QuestEvents.talkToEvent.Invoke(npcName); // Invoke the talkToEvent with the NPC's name

        Canvas confirmCanvas = confirmationUI.GetComponent<Canvas>();
        confirmationScript.EditDescription(dialogue[0]);
        confirmCanvas.enabled = true;
        confirmationUI.transform.Find("Button Yes").GetComponent<Button>().onClick.AddListener(LoadTowerDefenseScene);
    }

    public void LoadTowerDefenseScene()
    {
        SceneManager.LoadScene("TowerDefense");
    }

}
