using UnityEngine;
using UnityEngine.Events;


public class Tripwire : MonoBehaviour
{
    [Header("Tripwire")]
    [SerializeField] private string tripwireName = "FirstArea";
    bool isActive = false;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") && isActive)
            return;

        Debug.Log("Reached tripwire: " + tripwireName);
        QuestEvents.goToEvent.Invoke(tripwireName);

        isActive = true;
        gameObject.GetComponent<BoxCollider>().enabled = false;

    }
}
