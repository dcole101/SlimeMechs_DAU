using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

using System.Collections.Generic;

public class BossShakeReceiver : MonoBehaviour
{
    public CameraShake camShake;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Shake(float magnitude)
    {
        camShake.Shake(0.2f, magnitude);
    }

    public void LongShake(float magnitude)
    {
        camShake.Shake(2f, magnitude);
    }

    public void BeamShake(float magnitude)
    {
        camShake.Shake(5f, magnitude);
    }
}
