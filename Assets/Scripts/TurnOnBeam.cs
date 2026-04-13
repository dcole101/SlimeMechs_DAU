using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

public class TurnOnBeam : MonoBehaviour
{
    [SerializeField] private GameObject sludgeProjectile;  


    public void FireBeam()
    {
        if (sludgeProjectile != null)
            sludgeProjectile.SetActive(true);
    }

  
    public void StopBeam()
    {
        if (sludgeProjectile != null)
            sludgeProjectile.SetActive(false);
    }
}