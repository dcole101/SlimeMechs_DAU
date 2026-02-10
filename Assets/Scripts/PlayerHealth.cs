using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEvent onTakeDamage, onDeath;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakePlayerDamage(int damage)
    {
        Debug.Log("TakeDamage Player");
        currentHealth -= damage;
        onTakeDamage?.Invoke();

        if (currentHealth <= 0) Die();

        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Player died");
        //onDeath?.Invoke();
       
    }
}
