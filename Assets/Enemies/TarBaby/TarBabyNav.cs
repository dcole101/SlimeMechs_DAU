using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TarBabyNav : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Assign player Transform in Inspector

    [Header("Flee Settings")]
    public float fleeDistance = 10f;  // How far away from the player the enemy tries to be
    public float updateRate = 0.1f;   // How often to update destination (seconds)

    private NavMeshAgent agent;
    private float _updateTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null) return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            _updateTimer = updateRate;
            SetFleeDestination();
        }
    }

    private void SetFleeDestination()
    {
        // Direction from player to enemy (normalized)
        Vector3 awayFromPlayer = (transform.position - player.position).normalized;

        // Target point some distance away from player in that direction
        Vector3 desiredPosition = player.position + awayFromPlayer * fleeDistance;

        // Keep y the same as current to avoid weird vertical offsets
        desiredPosition.y = transform.position.y;

        // Send enemy there via NavMesh
        agent.SetDestination(desiredPosition);
    }
}
