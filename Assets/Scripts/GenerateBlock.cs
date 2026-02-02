using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GenerateBlock : MonoBehaviour
{
    [SerializeField] GameObject blockprefab;
    [SerializeField] bool isDebugging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDebugging = Application.isEditor && isDebugging;
        if(!isDebugging)
        {
Instantiate(blockprefab, transform.position, Quaternion.identity, transform);
   
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
