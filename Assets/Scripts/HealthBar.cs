using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
   
    public Image fill;  

    [Header("Colors")]
    public Gradient gradient;

    private int maxHealth;

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        SetHealth(health);
    }

    public void SetHealth(int health)
    {
        if (maxHealth <= 0) return;  // Prevent div by zero

        float normalizedValue = Mathf.Clamp01((float)health / maxHealth);
        fill.fillAmount = normalizedValue;
        fill.color = gradient.Evaluate(normalizedValue);
    
    }



    // Billboard stuff
    public Transform cam;
    public bool isWorldSpace;

    void LateUpdate()
    {
        if (isWorldSpace)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
