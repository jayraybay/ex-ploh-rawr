using TMPro;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float despawnZ = -5f;

    [Header("Nametag")]
    [SerializeField] private TMP_Text nametagText;

    private WallSpawner spawner;
    private bool isCorrect;
    private bool isMoving = true;
    private bool hasBeenHit;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        if (nametagText == null)
        {
            nametagText = GetComponentInChildren<TMP_Text>(true);
        }
    }

    public void Initialize(
        WallSpawner owner,
        WallAnswerData answer,
        float initialSpeed)
    {
        spawner = owner;
        isCorrect = answer.isCorrect;
        MoveSpeed = initialSpeed;

        if (nametagText != null)
        {
            nametagText.text = answer.answerText;
        }
        else
        {
            Debug.LogError(
                "WallObstacle: No TMP_Text found on the Nametag child.",
                this
            );
        }
    }

    private void Update() // WALL MOVEMENT. Move toward negative Z
    {
        if (!isMoving)
            return;

        
        transform.position +=
            Vector3.back * moveSpeed * Time.deltaTime;

        if (transform.position.z <= despawnZ)
        {
            if (spawner != null)
                spawner.UnregisterWall(this);

            Destroy(gameObject);
        }
    }

    public void Hit() // for sledgehammer
    {
        if (hasBeenHit)
            return;

        hasBeenHit = true;

        if (spawner != null)
        {
            spawner.OnWallHit(this, isCorrect);
        }

        if (isCorrect)
        {
            Destroy(gameObject);
        }
        else
        {
            // Prevents the repeated fail punishment from a same wall. The wall will remain active, and will continue moving
            hasBeenHit = false;
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void ResumeMovement()
    {
        isMoving = true;
    }
}
