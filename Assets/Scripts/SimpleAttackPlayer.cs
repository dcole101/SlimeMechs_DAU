using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;
using System.Collections.Generic;

public class SimpleAttackPlayer : MonoBehaviour
{
    public LayerMask whatIsPlayer;

    // Damage
    public int damage = 25;
    public float attackRange = 2f;

    public CameraShake camShake;

    void Start()
    {
        GameObject playerCam = GameObject.Find("PlayerFollowCamera");
        if (playerCam != null)
            camShake = playerCam.GetComponent<CameraShake>();
    }

    public void DeliverDamage() 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        for (int i = 0; i < hits.Length; i++)
        {
            PlayerHealth playerHealth = hits[i].GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakePlayerDamage(damage);
                //Shake(0.3f);
            }
        }

        Debug.Log($"Attack hit {hits.Length} players for {damage} damage");
    }

    public void DeliverDamageNoShake()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        for (int i = 0; i < hits.Length; i++)
        {
            PlayerHealth playerHealth = hits[i].GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakePlayerDamage(damage);
                
            }
        }

        Debug.Log($"Attack hit {hits.Length} players for {damage} damage");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    //public void Shake(float magnitude)
    //{
    //    camShake.Shake(0.2f, magnitude);
    //}

    //public void LongShake(float magnitude)
    //{
    //    camShake.Shake(0.4f, magnitude);
    //}
}