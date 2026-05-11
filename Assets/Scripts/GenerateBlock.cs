using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GenerateBlock : MonoBehaviour
{
    [SerializeField] GameObject blockprefab;
    [SerializeField] bool isDebugging;

    void Start()
    {
        isDebugging = Application.isEditor && isDebugging;
        if(!isDebugging)
        {
            Instantiate(blockprefab, transform.position, Quaternion.identity, transform);
   
        }
        
    }

}
