using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using System.Collections.Generic;

public class BossHealth : MonoBehaviour
{
   
    public int maxHealth;
    public HealthBar healthBar;  

    private int health;

    void Start()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Boss Damaged: " + damage);
        health -= damage;

        if (healthBar != null)
            healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
       
        Debug.Log("Boss defeated!");
        Destroy(gameObject, 0.5f); 
    }
}
