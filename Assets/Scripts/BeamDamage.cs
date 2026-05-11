using UnityEngine;

public class BeamDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public LayerMask whatIsPlayer = 1 << 6; 
    public float damage = 25f;
    public float damageInterval = 0.5f; 

    private float lastDamageTime;
    private PlayerHealth playerHealth; 

    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsPlayer) == 0) return;

        playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (Time.time >= lastDamageTime + damageInterval)
        {
            playerHealth.TakePlayerDamage((int)damage);
            lastDamageTime = Time.time;
            Debug.Log($"Beam hit player for {damage} damage");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            playerHealth = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.localPosition, transform.localScale * 0.9f);
    }
}