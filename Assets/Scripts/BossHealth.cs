using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class BossHealth : MonoBehaviour
{
    public int maxHealth = 1000; 
    [HideInInspector] public HealthBar healthBar; 
    [HideInInspector] public int health;

   
    private const float k_destroyDelay = 0.5f;
    private bool isDead = false;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Boss Damaged: " + damage);
        health -= damage;
        if (healthBar != null) healthBar.SetHealth(health);

        if (health <= 0) Die();
    }

    public virtual void Die()  
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        Debug.Log("Boss defeated!");
        Destroy(gameObject, k_destroyDelay);
        StartCoroutine(LoadEndScreenDelayed());
    }

    private IEnumerator LoadEndScreenDelayed()
    {
        yield return new WaitForSeconds(k_destroyDelay);
        SceneManager.LoadScene("EndScreen");
    }
}