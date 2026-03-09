using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 25;
    public Transform attackPoint;
    public float attackRange = 2f;
    public LayerMask enemyLayers;

    private bool canDamage = false;

    public void OnAttackStarted() 
    {
        Debug.Log("Attack Started Player");
        canDamage = true;
        Invoke(nameof(ResetDamage), 0.3f); 
    }

    public void DealDamage()  // Called at hit frame
    {
        Debug.Log("DealDamage Player");
   

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers, QueryTriggerInteraction.Collide);
        foreach (Collider enemy in hitEnemies)
        {
           
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            enemyAi?.EnemyTakesDamage(attackDamage);


            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            bossHealth?.TakeDamage(attackDamage); 

            Debug.Log("Enemy ai take damage called");
        }
    }

    private void ResetDamage()
    {
        canDamage = false;
    }
}
