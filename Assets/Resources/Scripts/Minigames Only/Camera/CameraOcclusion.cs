using UnityEngine;
using System.Collections;

public class CameraOcclusion : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerPos;
    public float transparencyAlpha = 0.3f;
    public float normalAlpha = 1.0f;

    void Start()
    {
        playerPos = GameObject.Find("Player Sprite").transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(playerPos.position); // The player on the screen
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(playerScreenPos);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Prop")) // Point a ray from main camera to the player model. If something obstructs the ray, determine what it is
        {
            GameObject collidedObj = hit.collider.gameObject;
            fadeObject(collidedObj);
        }
    }

    void fadeObject(GameObject go)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        renderer.enabled = false;
        /*
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = transparencyAlpha; // Alpha component of the object
            renderer.material.color = color;

            Debug.Log("Turned " + go + " to transparent.");
        }*/

    }

}
