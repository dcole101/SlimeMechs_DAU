
using UnityEngine;

public class TentacleHealth : MonoBehaviour
{
    public BossController boss; 

    public void NotifyBossDamage(int damage)
    {
        //if (boss != null)
        boss.BossTakeDamage(damage);
        Destroy(gameObject);
        Debug.Log("Tentacle Dead");
    }
}