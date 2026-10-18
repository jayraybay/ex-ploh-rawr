
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Button dashButton;
    [SerializeField] private MobileController mobileController; // joystick

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;

    private bool isDashing;
    private bool isOnCooldown;
    private Vector3 lastMoveDirection = Vector3.forward;

    private void Update()
    {
        if (mobileController == null)
            return;

        Vector3 input = new Vector3(
            mobileController.Horizontal(),
            0f,
            mobileController.Vertical()
        );

        if (input.sqrMagnitude > 0.01f)
            lastMoveDirection = input.normalized;
    }

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (characterController == null)
        {
            Debug.LogError("PlayerDash: CharacterController is missing. (Did you forget to add it on the Unity inspector via Player GameObject?)");
        }
    }
    
    
    private void OnEnable()
    {
        if (dashButton != null)
            dashButton.onClick.AddListener(Dash);
    }

    private void OnDisable()
    {
        if (dashButton != null)
            dashButton.onClick.RemoveListener(Dash);
    }

    


    public void Dash()
    {
        if (isDashing || isOnCooldown || characterController == null)
            return;

        StartCoroutine(PerformDash());
    }


    private IEnumerator PerformDash() // handles dash movement and its cooldown
    {
        isDashing = true;
        isOnCooldown = true;

        float elapsed = 0f;
        Vector3 dashVelocity = lastMoveDirection * (dashDistance / Mathf.Max(dashDuration, 0.01f));

        while (elapsed < dashDuration)
        {
            float step = Mathf.Min(Time.deltaTime, dashDuration - elapsed);
            characterController.Move(dashVelocity * step);
            elapsed += step;
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        isOnCooldown = false;
    }
}
