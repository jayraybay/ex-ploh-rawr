using UnityEngine;
using UnityEngine.UI;

public class Teleporter : NPC
{
    //------------[TELEPORTER LOGIC]-------------
    private GameObject entrance;
    private GameObject exit;
    private GameObject destination;
    private Collider entCollider;
    private Collider exitCollider;

    private GameObject player;
    private CharacterController playerController;
    //private Rigidbody rigb;

    /// <summary>
    /// Teleporters, both on entrance and exit, are functional using the current interact button.
    /// -Resets interact button after each teleport (This took me an hour jesus christ since OnTriggerExit and OnTriggerEnter are unity-engine-physics-based)
    /// </summary>


    //TODO: Player ALWAYS gets TP'd to the front of the door, not dynamically.
    //TODO: Player model is seen being TP'd slightly above the teleport location and falls momentarily. Loading screen/transition UI should cover this up.
    //TODO: Door sounds and door animations.

    void Start()
    {
        // GameObjects
        entrance = GameObject.Find("Entrance"); 
        exit = GameObject.Find("Exit");
        // Its colliders
        entCollider = entrance.GetComponent<Collider>();
        exitCollider = exit.GetComponent<Collider>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        //rigb = player.GetComponent<Rigidbody>();
        entrance.tag = "NPC";
        exit.tag = "NPC";

    }

    public override void Interact()
    {
        Teleport(gameObject);
        //Debug.Log("Teleporter: " + gameObject);
    }

    public void Teleport(GameObject receiver)
    {
        if (receiver == null)
        {
            Debug.LogError("Receiver is null. Cannot teleport.");
            return;
        }
        else if (receiver == entrance)
        {
            destination = exit;
        }
        else if (receiver == exit)
        {
            destination = entrance;
        }
        else
        {
            Debug.LogError("Receiver is not a valid teleporter.");
            return;
        }

        //Debug.Log("Attempting to teleport to exit..");
        isTeleporting(true);

        player.transform.position = destination.transform.position + new Vector3(0, 0, -2); // teleports + moves player to the front

        isTeleporting(false);
        Physics.SyncTransforms();

        PlayerInteract ps = player.GetComponent<PlayerInteract>();
        ps.ForceExit(); //FORCES ONTRIGGEREXIT TO RESET INTERACT BUTTON
    }

    public void isTeleporting(bool state)
    {
        state = !state;
        playerController.enabled = state;
        entCollider.enabled = state;
        exitCollider.enabled = state;
    }
}