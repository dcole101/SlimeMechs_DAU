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

    public void DealDamage()
    {
        Debug.Log("DealDamage Player");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers, QueryTriggerInteraction.Collide);

        foreach (Collider enemy in hitEnemies)
        {
           
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            enemyAi?.EnemyTakesDamage(attackDamage);


            BossController bossHealth = enemy.GetComponent<BossController>();
            bossHealth?.BossTakeDamage(attackDamage);

          
            TentacleHealth tentHealth = enemy.GetComponent<TentacleHealth>();
            tentHealth?.NotifyBossDamage(attackDamage);

            TarBabyNav babyHealth = enemy.GetComponent<TarBabyNav>();
            babyHealth?.TakeDamage(attackDamage);

            Debug.Log("Damage applied to: " + enemy.name);

           
        }
    }

    private void ResetDamage()
    {
        canDamage = false;
    }
}
