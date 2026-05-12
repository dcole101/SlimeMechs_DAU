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

    public HealthBar healthBar;
    [SerializeField] private BossController boss;

    public Animator babyanimator;
    private bool isDead;

    private bool fleePointSet;
    public float walkPointRange = 10f;  // Range to search for flee points
    private Vector3 fleePoint;

    void Start()
    {
        babyanimator = GetComponent<Animator>();
        isDead = false;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        if (isDead == false)
        {
            _updateTimer -= Time.deltaTime;
            if (_updateTimer <= 0f)
            {
                _updateTimer = updateRate;
                if (!fleePointSet) SearchFleePoint();
            }

            Patrolling();
        }
    }

    private void Patrolling()
    {
        if (!fleePointSet) SearchFleePoint();

        if (fleePointSet)
        {
            agent.SetDestination(fleePoint);

            Vector3 distanceToFleePoint = transform.position - fleePoint;

            if (distanceToFleePoint.magnitude < 1f || !agent.hasPath)
            {
                fleePointSet = false;
            }
        }
    }

    private void SearchFleePoint()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 awayFromPlayer = (transform.position - player.position).normalized;

            float randomDistance = Random.Range(walkPointRange * 0.5f, walkPointRange);
            Vector3 randomOffset = awayFromPlayer * randomDistance;

            Vector3 randomPoint = new Vector3(
                transform.position.x + randomOffset.x + Random.Range(-2f, 2f),
                transform.position.y,
                transform.position.z + randomOffset.z + Random.Range(-2f, 2f)
            );

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                float distanceFromPlayer = Vector3.Distance(hit.position, player.position);
                if (distanceFromPlayer > 5f) 
                {
                    fleePoint = hit.position;
                    fleePointSet = true;
                    return;
                }
            }
        }

        fleePointSet = false;
        Debug.LogWarning("No valid flee point found away from player!");
    }

    public void TakeDamage(int damage)
    {
        BossController boss = FindObjectOfType<BossController>();
        if (boss != null)
            boss.BossTakeDamage(damage);
        else
            Debug.LogError("No BossController found!");

        babyanimator.SetTrigger("TakeHit");
    }

    public void Setup(Transform playerRef)
    {
        player = playerRef;
    }

    public void Die()
    {
        //isDead = true;
        if (agent != null)
        {
            isDead = true;
            agent.isStopped = true;  
            agent.ResetPath();           
            //agent.enabled = false;       
        }
        Debug.Log("TarBaby Disabled");
    }


}
