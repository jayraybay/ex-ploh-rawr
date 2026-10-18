using System;
using UnityEngine;

public class NPC_Interaction : NPC
{

    int index = 0;
    string[] dialogue = {
        "Hello there, try solving those terminals to proceed.",
        "If you ever failed, you can try again.",
    };
    
    string npcName = "Bun"; // Helps Quest system identify which NPC was interacted with

    [Header("Tripwire")]
    [SerializeField] private string tripwireName = "FirstArea"; // yes. the tripwire is also attached here.
    bool isActive = false;

    new void Start(){}
 
    new void Update()
    {
        base.Update();

        
    }

    // This function is called when a player interacted this NPC
    public override void Interact() // Changed it to protected override for Quest system to work properly
    {
        if (index >= dialogue.Length) index = 0;
        Speak(dialogue[index]);
        index++;
        QuestEvents.talkToEvent.Invoke(npcName); // Invoke the talkToEvent with the NPC's name
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isActive)
            return;

        

        Debug.Log("Reached tripwire: " + tripwireName);
        QuestEvents.goToEvent.Invoke(tripwireName);

        isActive = true; // prevents from firing more than once
    }

}
