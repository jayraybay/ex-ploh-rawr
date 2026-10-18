using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomEncounters : MonoBehaviour
{
    private GameObject player;
    private GameObject battleground;
    private Collider bgCollider;
    private PlayerInteract piScript;


    private float distanceUntilNextAttempt;
    private float distanceTravelled;
    private float distanceMoved;
    private int encounterRoll;
    private int targetAmount;

    private Vector3 lastPosition;
    private bool isMoving;

    //DEBUG UI
    private TextMeshProUGUI totalDistanceText;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        battleground = GameObject.Find("Battleground");
        bgCollider = GameObject.Find("Battleground").GetComponent<Collider>();
        battleground.tag = "Battleground";
        piScript = player.GetComponent<PlayerInteract>();
        lastPosition = player.transform.position;
        totalDistanceText = GameObject.Find("TotalDistance").GetComponent<TextMeshProUGUI>();

        //Debug.Log(bgCollider.gameObject);
        distanceUntilNextAttempt = UnityEngine.Random.Range(500f, 2000f);
        //Debug.Log("Distance until next attempt: " + distanceUntilNextAttempt);
        totalDistanceText.text = "Total Distance Travelled: " + distanceTravelled + " / " + distanceUntilNextAttempt;
        targetAmount = 10; //guaranteed encounter on first attempt

        totalDistanceText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //attemptEncounter();
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log(other + " has entered battleground!");
            totalDistanceText.gameObject.SetActive(false);
        }
    } //When player stops touching battleground, hide the distance travelled text.

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log(other + " is staying in battleground!");
            if (player.transform.position != lastPosition)
            { // Detects if player is moving, if yes, capture its position and accumulate distance travelled.
                lastPosition = player.transform.position;
                distanceTravelled += Vector3.Distance(transform.position, lastPosition);
                
                totalDistanceText.text = "Total Distance Travelled: " + distanceTravelled + " / " + distanceUntilNextAttempt; //DEBUG
                totalDistanceText.gameObject.SetActive(true);

                if (distanceTravelled >= distanceUntilNextAttempt)
                { //If player reaches certain distance, roll a dice, then reset the values to prepare for another one.
                    distanceTravelled = 0;
                    distanceUntilNextAttempt = UnityEngine.Random.Range(100f, 500f);
                    //Debug.Log("Distance until next attempt: " + distanceUntilNextAttempt);
                    encounterRoll = UnityEngine.Random.Range(1, 11); // Roll a dice from 1 to 10
                    

                    if (targetAmount >= encounterRoll)
                    { 
                        Debug.Log("Encounter rolled! " + targetAmount + " / " + encounterRoll);
                        targetAmount = 1; // Reset amount
                    }
                    else if (targetAmount < encounterRoll)
                    {
                        Debug.Log("No encounter rolled. Increasing chance... " + targetAmount + " / " + encounterRoll);
                        targetAmount += 1;
                    }
                }
            }
        }
    }
}

