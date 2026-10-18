using UnityEngine;

public class RepairSyntaxHammerAttack : MonoBehaviour
{
    // adjusted the hitbox to make the hit more accurate to where player is standing
    private bool hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root)
        {
            return; 
        }
        if (hit)
        {
            return;
        }

        Debug.Log("Repair Syntax hammer collided with: " + other.name);
        CodeLine codeLine = other.GetComponentInParent<CodeLine>();

        if (codeLine == null)
        {
            Debug.Log("No CodeLine found on the collided object.");
            return;
        }

        Debug.Log("CodeLine found. Calling Hit().");
        hit = true;
        codeLine.Hit();
    }
}
