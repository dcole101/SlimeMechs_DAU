
using UnityEngine;

public class TentacleHealth : MonoBehaviour
{
    public BossController boss;
    public GameObject SingleTentacleAnimator;
    private Animator tentacleanimator;
    int tentacleHits = 0;

    void Start()
    {
        tentacleanimator = SingleTentacleAnimator.GetComponent<Animator>();
    }

    public void NotifyBossDamage(int damage)
    {
        //if (boss != null)
        boss.BossTakeDamage(damage);
        //Destroy(gameObject);
        Debug.Log("Tentacle Hit");

        tentacleHits++;

        if (tentacleHits >= 2)
        {

            tentacleanimator.SetTrigger("Die");
            Destroy(gameObject, 2f);
            Debug.Log("Tentacle Dead");
        }
        else
        {

            tentacleanimator.SetTrigger("TakeHit");
        }
    }
}