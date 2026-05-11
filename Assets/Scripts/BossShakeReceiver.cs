using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

using System.Collections.Generic;

public class BossShakeReceiver : MonoBehaviour
{
    public CameraShake camShake;

    public void Shake(float magnitude)
    {
        camShake.Shake(0.2f, magnitude);
    }

    public void LongShake(float magnitude)
    {
        camShake.Shake(0.8f, magnitude);
    }

    public void BeamShake(float magnitude)
    {
        camShake.Shake(5f, magnitude);
    }
}
