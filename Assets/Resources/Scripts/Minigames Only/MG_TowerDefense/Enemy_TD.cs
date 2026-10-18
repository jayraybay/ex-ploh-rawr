using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;


public class Enemy_TD : NPC
{
    // Note: This class is responsible for an individual enemy's AI. TD stands for "Tower Defense"

    // Fetch References
    NavMeshAgent agent;
    public Transform target;
    public GameObject tower;
    public string npcName = "EvilBun";
    public int enemyID = 0 ;// Helps slash() identify which NPC     to attack

    [Header("Enemy Properties")]
    public float health = 25;
    public bool isEvil;

    [Header("References")]
    [SerializeField] private Transform playerChest;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tower towerScript;
    [SerializeField] private Enemy_TD enemyScript;
    [SerializeField] private float totalDamageDone;

    private Button buttonAttack;
    private Rigidbody rb;
    private Coroutine tickCoroutine;
    public UnityAction slashAction;



    void Start()
    {
        // Fetch references
        target = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        buttonAttack = GameObject.FindGameObjectWithTag("UI_Attack").GetComponent<Button>();
        rb = GetComponent<Rigidbody>();
        towerScript = GameObject.FindWithTag("Tower").GetComponent<Tower>();
        tower = GameObject.FindWithTag("Tower");
        totalDamageDone = 0;

        /*
        slashAction = new UnityAction(slash);
        slashAction = slash;
        slashAction?.Invoke();
       */

        // WIP
        if (tower != null)
        {
            target = tower.transform;
            towerScript = tower.GetComponent<Tower>();
        }
        else
        {
            Debug.LogError("Tower could not be found!");
            return;
        }

        changeTarget(target);       // 1. Identify the target for the enemy to move towards
        snapToNavMesh();            // 2. Snap the enemy to the NavMesh after it has been spawned
        moveToTarget(target);       // 3. Move the enemy to the target after it has been identified and snapped to the NavMesh
    }

    void Update()
    {
        base.Update();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Tower"))
            return;

        if (tickCoroutine == null) // Take tower damage every x seconds if it detects collider attached with a tag "Enemy_TD"
        {
            tickCoroutine = StartCoroutine(TickRoutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            if (tickCoroutine != null)
            {
                StopCoroutine(tickCoroutine);
                tickCoroutine = null;
            }
        }
    }

    public void snapToNavMesh()
    {
        if (NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit hit,
            5.0f,
            NavMesh.AllAreas))
        {
            try 
            {
                agent.Warp(hit.position); // Suppose the enemy has already spawned, snap it to the nearest point on the NavMesh so it can move
            } catch (MissingComponentException e)
            {
                Debug.LogError(
                    $"{gameObject.name} is missing a NavMeshAgent component! " +
                    "(Did you add a NavMeshAgent to the prefab?)"
                );
            }
        }
        else
        {
            Debug.LogError(
                $"{gameObject.name} is too far away from a baked NavMesh! " +
                "(Did you bake a NavMesh yet?)"
            );
        }
    }

    public void moveToTarget(Transform t) // Tells AI where to move
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(t.position);
        }
        else if (target == null)
        {
            Debug.LogError(
                $"{gameObject.name} has no target to move to!"
            );
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                $"{gameObject.name} is not on a NavMesh! " +
                "(Did you bake a NavMesh yet?)"
            );
        }
    }

    public void changeTarget(Transform t)
    {
        target = t; // Change target
    }

    public void takeDamage() // Void: Receive damage for this individual NPC. NOT Tower.
    {
        health -= towerScript.playerDamage; // One hit before death
        Debug.Log($"{gameObject.name} took damage! {health}-hp remaining.");
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
            Destroy(gameObject);
            towerScript.enemyDefeated();
        }
        //TODO: Knockback effect when taking damage (OPT)
        //TODO: Critical chance shakes the screen and deals more damage  (OPT)
        //TODO: Add nametag to enemy prefab and change it based on answer

        if (!isEvil)
        {
            towerScript.towerHealth -= towerScript.allySlainDmg;
            towerScript.updateHpBar();
            Debug.LogWarning("You've hit an ALLY! OUCH!");
        }

    }

    public void allyTowerTouch() // What further happens to an ally (fake enemy) if it touches the tower
    {
        Destroy(gameObject); // individual ally disappears
        towerScript.enemyDefeated();
        towerScript.updateHpBar();
        // TODO: Instantiate VFX for healing
    }



    IEnumerator TickRoutine() 
    {
        while (towerScript != null && towerScript.isOngoing) // Damage handler AGAINST tower
        {
            if (!isEvil) // Logic if ally touches the tower
            {
                towerScript.towerHealth += towerScript.allyHeal; // Heals the tower
                allyTowerTouch();
            } else if (isEvil) // Logic if enemy touches the tower
            {
                float damage = towerScript.takeDamage();

                //Debug.Log($"Damage! {damage}-hp");

                towerScript.towerHealth -= damage;
                totalDamageDone += damage;
                towerScript.updateHpBar();
            }

            yield return new WaitForSeconds(towerScript.enemyDamageTickRate); // Parameter is AKA enemy attack speed. CRASHES IF REMOVED AT THE END OF WHILE LOOP NIGGA 
        }

        tickCoroutine = null;
    }
}