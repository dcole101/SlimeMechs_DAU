//using UnityEngine;
//using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
//public class TarBabyNav : MonoBehaviour
//{
//    [Header("References")]
//    public Transform player;

//    [Header("Flee Settings")]
//    public float fleeDistance = 15f;
//    public float updateRate = 0.1f;

//    private NavMeshAgent agent;
//    private float _updateTimer;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//    }

//    void Update()
//    {
//        if (player == null) return;

//        _updateTimer -= Time.deltaTime;
//        if (_updateTimer <= 0f)
//        {
//            _updateTimer = updateRate;
//            SetFleeDestination();
//        }
//    }

//    void SetFleeDestination()
//    {
//        Vector3 awayFromPlayer = (transform.position - player.position).normalized;
//        Vector3 desiredPosition = player.position + awayFromPlayer * fleeDistance;
//        desiredPosition.y = transform.position.y;
//        agent.SetDestination(desiredPosition);
//    }

//    public void Setup(Transform playerRef)
//    {
//        player = playerRef;
//    }

//    public void TakeDamage(int damage)
//    {
//        BossHealth health = GetComponent<BossHealth>();
//        if (health != null) health.TakeDamage(damage);
//    }
//}


using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TarBabyNav : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Flee Settings")]
    public float fleeDistance = 10f;
    public float updateRate = 0.1f;

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

    public void TakeDamage(int damage)
    {
        BossController health = GetComponent<BossController>();
        if (health != null) health.BossTakeDamage(damage);
    }

    public void Setup(Transform playerRef)
    {
        player = playerRef;
    }
}
