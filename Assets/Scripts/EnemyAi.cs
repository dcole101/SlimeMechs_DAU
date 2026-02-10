using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public GameObject playerObj;

    public LayerMask whatGround, whatPlayer;

    public int health;
    public HealthBar healthBar;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, attackRange;

    // New: how far before we give up chasing/attacking
    public float loseInterestRange = 30f;

    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = playerObj.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatPlayer);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If player is too far, stop any chasing/attacking and go back to patrol
        if (distanceToPlayer > loseInterestRange)
        {
            Patrolling();
            return;
        }

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distancetoWalkPoint = transform.position - walkPoint;

        // Walk point reached
        if (distancetoWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);

        // Actually on ground?
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            // Attacking!!
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void EnemyTakesDamage(int damage)
    {
        Debug.Log("Enemy Damaged: " + damage);
        health -= damage;

        //health bar
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
