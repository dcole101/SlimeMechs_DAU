using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    // based off custom bullet youtube video (part of Navmesh Enemies)
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsPlayer;

    // Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    // Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    // Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicsMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // When to explode
        if (collisions > maxCollisions) Explode();

        // Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
      
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        // Check for player in explosion radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
        for (int i = 0; i < hits.Length; i++)
        {
            PlayerHealth playerHealth = hits[i].GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakePlayerDamage(explosionDamage);
            }

            // Add explosion force (if player has a rigidbody)
            Rigidbody hitRb = hits[i].GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }

       
        Invoke(nameof(Delay), 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.collider.CompareTag("Bullet")) return;

        collisions++;

    
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakePlayerDamage(explosionDamage);
            }

            if (explodeOnTouch)
            {
                Explode();
            }
        }
        else
        {
            
            if (explodeOnTouch)
            {
                Explode();
            }
        }
    }

    private void Setup()
    {
        
        physics_mat = new PhysicsMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        GetComponent<SphereCollider>().material = physics_mat;

        rb.useGravity = useGravity;
    }

   
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
