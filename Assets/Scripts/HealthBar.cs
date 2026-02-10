using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        gradient.Evaluate(1f);
    }




    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    //billboard stuff
    public Transform cam;
    public bool isWorldSpace;

    void LateUpdate()
    {
        if(isWorldSpace)
        {
            transform.LookAt(transform.position + cam.forward);
        }
       
    }
    
 
    
}
