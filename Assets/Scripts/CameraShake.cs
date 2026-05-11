using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin perlinNoise;
    public float shakeDuration = 0.3f;
    public float maxShakeIntensity = 2f; 

    void Awake()
    {
      
        perlinNoise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (perlinNoise == null)
        {
            Debug.LogError("No CinemachineBasicMultiChannelPerlin found!");
        }
    }

    public void Shake(float duration = 0.1f, float magnitude = 1f)
    {
        StartCoroutine(ShakeCoroutine(duration, maxShakeIntensity * magnitude));
    }

   
    public void Shake(float magnitude)
    {
        Shake(0.2f, magnitude);
    }

  


    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float startIntensity = perlinNoise.m_AmplitudeGain;
        float elapsed = 0f;

     
        while (elapsed < duration * 0.5f)
        {
            perlinNoise.m_AmplitudeGain = Mathf.Lerp(startIntensity, intensity, elapsed / (duration * 0.5f));
            elapsed += Time.deltaTime;
            yield return null;
        }

       
        float decayStart = elapsed;
        while (elapsed < duration)
        {
            perlinNoise.m_AmplitudeGain = Mathf.Lerp(intensity, startIntensity, (elapsed - decayStart) / (duration * 0.5f));
            elapsed += Time.deltaTime;
            yield return null;
        }

     
        perlinNoise.m_AmplitudeGain = startIntensity;
    }
}