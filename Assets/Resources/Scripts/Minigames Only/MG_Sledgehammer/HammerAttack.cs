
using UnityEngine;

public class SledgehammerAttack : MonoBehaviour
{
    private bool hit;
    // NOTE: I increased the collider radius (hitbox). It's larger than VFX_Slash's hitbox (on Tower Defense). It's not precise, but it works on these walls.
    private void OnTriggerEnter(Collider other) 
    {
        if (hit)
            return;

        Debug.Log("Sledgehammer collided with: " + other.name);

        WallObstacle wall = other.GetComponentInParent<WallObstacle>();

        if (wall == null)
        {
            Debug.Log("No WallObstacle found on the collided object.");
            return;
        }

        Debug.Log("WallObstacle found. Calling Hit().");

        hit = true;
        wall.Hit();
    }
}
