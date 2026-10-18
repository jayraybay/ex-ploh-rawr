using UnityEngine;

public class BillboardParallel : MonoBehaviour
{
    // Attach this script to any GameObject that uses 2D sprites or UI elements that you want to always face the camera.
    private Transform camTransform;

    [Header("Settings")]
    [SerializeField] private bool lockElementRotation;
    [SerializeField] private bool tiltToCamera;
    [SerializeField] private bool flipped;

    void Start()
    {
        lockElementRotation = false;
        tiltToCamera = false;
        flipped = false; // Declare default values in start() to stop the element from flipping left and right (seizure) whenever a value is overridden by the hierarchy's serializefield feature.

        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No main camera found. (Have you tagged the main camera?)");
        }

    }

    void LateUpdate()
    {
        if (camTransform == null) return;

        if (lockElementRotation)
        {
            // These keep the sprite perfectly vertical
            Vector3 targetPosition = camTransform.position;
            targetPosition.y = transform.position.y;
            if (tiltToCamera)
            {
                transform.LookAt(targetPosition); // Tilts the sprite to match the camera's angle
            }
        }
        if (flipped) // I put this here so it can be configured during runtime using the inspector
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1; // Mirror/flip the element horizontally
            transform.localScale = scale;
        }
        else
        {
            // Matches camera angle
            transform.rotation = camTransform.rotation;
        }
    }
}
