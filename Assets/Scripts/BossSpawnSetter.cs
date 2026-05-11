using UnityEngine;
using System.Collections;

public class BossSpawnSetter : MonoBehaviour
{
  
    public BossController bossController;
    public TileManager tileManager;
    public CapsuleCollider bosshitcollider;
    public GameObject babylocation;
    public GameObject explosion;

    public float setDelay = 2f;

    void Start()
    {
        if (bossController == null)
            bossController = FindObjectOfType<BossController>();

        StartCoroutine(SetBossSpawnPosition());
    }

    private IEnumerator SetBossSpawnPosition()
    {
        yield return new WaitForSeconds(setDelay);

        GameObject tileBoss = GameObject.Find("TileBoss(Clone)");
        if (tileBoss != null)
        {
            bossController.spawnPosition = tileBoss.transform.position;
            bosshitcollider.center = tileBoss.transform.position;
            babylocation.transform.position = tileBoss.transform.position;


            Vector3 targetPos = tileBoss.transform.position;
            targetPos.y = 1.6f;
            explosion.transform.position = targetPos;
         

            Debug.Log($"Boss spawn set to TileBoss(Clone) at: {tileBoss.transform.position}");
        }
        else
        {
            Debug.LogWarning("TileBoss(Clone) not found!");
        }
    }
}