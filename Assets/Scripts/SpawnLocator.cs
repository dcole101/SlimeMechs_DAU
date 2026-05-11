//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections.Generic;

//public class SpawnLocator : MonoBehaviour
//{
//    [SerializeField] BossController bossController;
//    [SerializeField] string bossSpawnName = "BossSpawnPoint_10";

//    [Header("Tile Replacement")]

//    [SerializeField] GameObject replacementPrefab; 

//    void Start()
//    {
//        FindAndSetBossSpawn();
//    }

//    public void FindAndSetBossSpawn()
//    {
//        List<GameObject> spawnPoints = new List<GameObject>();
//        Scene scene = SceneManager.GetActiveScene();
//        GameObject[] roots = scene.GetRootGameObjects();

//        foreach (GameObject root in roots)
//        {
//            FindAllNamed(root, "Tile 15(Clone)", spawnPoints);
//        }

//        if (spawnPoints.Count > 0)
//        {

//            GameObject bossSpawnEmpty = new GameObject(bossSpawnName);
//            bossSpawnEmpty.transform.position = spawnPoints[0].transform.position;

//            if (bossController == null)
//                bossController = FindObjectOfType<BossController>();
//            bossController.spawnPosition = bossSpawnEmpty.transform.position;

//            Debug.Log($"Created {bossSpawnName} at Tile 15 position: {bossSpawnEmpty.transform.position}");
//        }
//        else
//        {
//            Debug.LogWarning("No Tile 15(Clone) found.");
//        }
//    }

//    public void ReplaceTile15()
//    {
//        List<GameObject> tile15List = new List<GameObject>();
//        Scene scene = SceneManager.GetActiveScene();
//        GameObject[] roots = scene.GetRootGameObjects();

//        foreach (GameObject root in roots)
//        {
//            FindAllNamed(root, "Tile 15(Clone)", tile15List);
//        }

//        if (tile15List.Count > 0 && replacementPrefab != null)
//        {
//            GameObject targetTile = tile15List[0];  
//            Vector3 spawnPos = targetTile.transform.position;
//            Quaternion spawnRot = targetTile.transform.rotation;
//            Transform parent = targetTile.transform.parent;


//            DestroyImmediate(targetTile);

//            // Instantiate replacement
//            GameObject newTile = Instantiate(replacementPrefab, spawnPos, spawnRot, parent);
//            newTile.name = "Tile 15(Clone)"; 

//            Debug.Log($"Replaced Tile 15 with {replacementPrefab.name} at {spawnPos}");
//        }
//        else
//        {
//            Debug.LogWarning("No Tile 15 found or replacement prefab not assigned.");
//        }
//    }

//    private void FindAllNamed(GameObject parent, string name, List<GameObject> results)
//    {
//        if (parent.name == name)
//            results.Add(parent);

//        for (int i = 0; i < parent.transform.childCount; i++)
//        {
//            FindAllNamed(parent.transform.GetChild(i).gameObject, name, results);
//        }
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BossSpawnLocator : MonoBehaviour
{
    [SerializeField] BossController bossController;
    [SerializeField] string bossSpawnName = "BossSpawnPoint_10";

    void Start()
    {
        FindAndSetBossSpawn();
    }

    public void FindAndSetBossSpawn()
    {
        GameObject bossSpawnCoord = FindObject("TileBoss(Clone)");

        if (bossSpawnCoord != null)
        {
            GameObject bossSpawnEmpty = new GameObject(bossSpawnName);
            bossSpawnEmpty.transform.position = bossSpawnCoord.transform.position;

            if (bossController == null)
                bossController = FindObjectOfType<BossController>();
            bossController.spawnPosition = bossSpawnEmpty.transform.position;

            Debug.Log($"Created {bossSpawnName} at BossSpawnCoord position: {bossSpawnEmpty.transform.position}");
        }
        else
        {
            Debug.LogWarning("No TileBoss(Clone) found.");
        }
    }

    private GameObject FindObject(string targetName)
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            GameObject found = FindNamed(root, targetName);
            if (found != null)
                return found;
        }
        return null;
    }

    private GameObject FindNamed(GameObject parent, string name)
    {
        if (parent.name == name)
            return parent;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject found = FindNamed(parent.transform.GetChild(i).gameObject, name);
            if (found != null)
                return found;
        }
        return null;
    }
}