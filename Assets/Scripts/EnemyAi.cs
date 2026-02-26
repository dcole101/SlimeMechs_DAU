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

            Vector3 distancetoWalkPoint = transform.position - walkPoint;

            // Walk point reached OR agent is stuck (no path)
            if (distancetoWalkPoint.magnitude < 1f || !agent.hasPath)
            {
                walkPointSet = false;
            }
        }
    }


    private void SearchWalkPoint()
    {
        // Try up to 30 times to find a valid NavMesh point
        for (int i = 0; i < 30; i++)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            Vector3 randomPoint = new Vector3(
                transform.position.x + randomX,
                transform.position.y,
                transform.position.z + randomZ
            );

            NavMeshHit hit;
            // SamplePosition finds the *closest valid NavMesh position* within maxDistance (10f)
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                walkPointSet = true;
                return; // Found a valid point!
            }
        }

        // Fallback: if no valid point found, stay put
        walkPointSet = false;
        Debug.LogWarning("No valid walk point found within range!");
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
