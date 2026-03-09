using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public RectTransform canvasTransform;
    public RectTransform indicatorPrefab;      

    [Header("Settings")]
    public string enemyTag = "Enemy";
    public float edgeOffset = 50f;

    class EnemyIndicator
    {
        public Transform enemy;
        public RectTransform arrow;
    }

    readonly List<EnemyIndicator> _indicators = new List<EnemyIndicator>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject e in enemies)
        {
            Debug.Log("Indicator: there is " + e + "enemies");
            CreateIndicatorFor(e.transform);
        }
    }

    void CreateIndicatorFor(Transform enemy)
    {
        RectTransform arrowInstance =
            Instantiate(indicatorPrefab, canvasTransform);

        arrowInstance.gameObject.SetActive(true);

        _indicators.Add(new EnemyIndicator
        {
            enemy = enemy,
            arrow = arrowInstance
        });

        Debug.Log("Indicator: created indicator");
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        for (int i = _indicators.Count - 1; i >= 0; i--)
        {
            var entry = _indicators[i];

            if (entry.enemy == null)
            {
                if (entry.arrow != null)
                    Destroy(entry.arrow.gameObject);

                _indicators.RemoveAt(i);
                continue;
            }

            UpdateIndicator(entry);
        }
    }

    void UpdateIndicator(EnemyIndicator entry)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(entry.enemy.position);

        if (screenPos.z < 0f)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
            screenPos.z = 0.1f;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 fromCenter = (Vector2)screenPos - screenCenter;
        Vector2 dir = fromCenter.normalized;

        bool isOnScreen =
            screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height;

        Vector2 indicatorPos;

        if (isOnScreen)
        {
            indicatorPos = screenPos;
        }
        else
        {
            float halfW = Screen.width / 2f - edgeOffset;
            float halfH = Screen.height / 2f - edgeOffset;

            float t = Mathf.Min(
                Mathf.Abs(halfW / dir.x),
                Mathf.Abs(halfH / dir.y)
            );

            indicatorPos = screenCenter + dir * t;
        }

        entry.arrow.position = indicatorPos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        entry.arrow.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
