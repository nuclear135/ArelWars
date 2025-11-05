using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArelWars.Units;

// Up/Down 두 라인의 진행도를 축약해 보여주는 미니맵
// - 각 라인에 가로 막대를 두고, 유닛/영웅을 작은 점으로 표시
// - X좌표를 타워-타워 사이로 정규화하여 0~1 값으로 위치시킨다
public class LaneMinimap : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private RectTransform laneUpBar;
    [SerializeField] private RectTransform laneDownBar;
    [SerializeField] private RectTransform dotPrefab;

    [Header("타워 참조")]
    [SerializeField] private Transform playerTower;
    [SerializeField] private Transform enemyTower;

    [Header("표시 옵션")]
    [SerializeField] private float edgePadding = 6f;
    [SerializeField] private Color playerUnitColor = new Color(0.2f, 0.8f, 1f);
    [SerializeField] private Color playerHeroColor = new Color(0f, 1f, 1f);
    [SerializeField] private Color enemyUnitColor = new Color(1f, 0.45f, 0.35f);
    [SerializeField] private Color enemyHeroColor = new Color(1f, 0.7f, 0.2f);
    [SerializeField] private Color towerColor = Color.white;
    [SerializeField] private float unitDotSize = 8f;
    [SerializeField] private float heroDotSize = 12f;
    [SerializeField] private float towerDotSize = 10f;

    private readonly List<RectTransform> upDots = new List<RectTransform>();
    private readonly List<RectTransform> downDots = new List<RectTransform>();

    private void Start()
    {
        // 타워 점(양끝)을 미리 배치
        PlaceTowerDots();
    }

    private void Update()
    {
        // 매 프레임 유닛/영웅을 스캔하여 점을 업데이트
        UpdateLane(Line.Up, laneUpBar, upDots);
        UpdateLane(Line.Down, laneDownBar, downDots);
    }

    private void PlaceTowerDots()
    {
        if (laneUpBar == null || laneDownBar == null || dotPrefab == null || playerTower == null || enemyTower == null)
        {
            return;
        }

        float left = Mathf.Min(playerTower.position.x, enemyTower.position.x);
        float right = Mathf.Max(playerTower.position.x, enemyTower.position.x);

        CreateTowerDotOnBar(laneUpBar, left, right);
        CreateTowerDotOnBar(laneUpBar, right, right);
        CreateTowerDotOnBar(laneDownBar, left, right);
        CreateTowerDotOnBar(laneDownBar, right, right);
    }

    private void CreateTowerDotOnBar(RectTransform bar, float towerX, float maxX)
    {
        RectTransform dot = Instantiate(dotPrefab, bar);
        Image img = dot.GetComponent<Image>();

        if (img != null)
        {
            img.color = towerColor;
        }

        SetDotSize(dot, towerDotSize);

        // 타워 위치는 양끝으로 스냅
        float x;
        if (towerX == maxX)
        {
            x = bar.rect.size.x - edgePadding;
        }
        else
        {
            x = edgePadding;
        }

        dot.anchoredPosition = new Vector2(x, 0f);
    }

    private void UpdateLane(Line line, RectTransform bar, List<RectTransform> pool)
    {
        if (bar == null || dotPrefab == null || playerTower == null || enemyTower == null)
        {
            return;
        }

        // 간단하게 FindObjectsOfType로 유닛을 수집 (규모 커지면 등록/해제 방식 권장)
        UnitController2D[] all = Object.FindObjectsOfType<UnitController2D>();

        int used = 0;

        float left = Mathf.Min(playerTower.position.x, enemyTower.position.x);
        float right = Mathf.Max(playerTower.position.x, enemyTower.position.x);
        float span = Mathf.Max(0.0001f, right - left);

        foreach (UnitController2D u in all)
        {
            if (u.line != line)
            {
                continue;
            }

            // 0~1 진행도 계산
            float t = Mathf.InverseLerp(left, right, u.transform.position.x);

            RectTransform dot = GetFromPool(pool, ref used, bar);
            Image img = dot.GetComponent<Image>();

            if (img != null)
            {
                if (u.isPlayer)
                {
                    if (u.isHero)
                    {
                        img.color = playerHeroColor;
                    }
                    else
                    {
                        img.color = playerUnitColor;
                    }
                }
                else
                {
                    if (u.isHero)
                    {
                        img.color = enemyHeroColor;
                    }
                    else
                    {
                        img.color = enemyUnitColor;
                    }
                }
            }

            if (u.isHero)
            {
                SetDotSize(dot, heroDotSize);
            }
            else
            {
                SetDotSize(dot, unitDotSize);
            }

            float x = Mathf.Lerp(edgePadding, bar.rect.size.x - edgePadding, t);
            dot.anchoredPosition = new Vector2(x, 0f);
            dot.gameObject.SetActive(true);
        }

        // 사용하지 않은 점은 숨기기
        for (int i = used; i < pool.Count; i++)
        {
            pool[i].gameObject.SetActive(false);
        }
    }

    private RectTransform GetFromPool(List<RectTransform> pool, ref int used, RectTransform parent)
    {
        if (used < pool.Count)
        {
            RectTransform r = pool[used];
            used += 1;
            return r;
        }
        else
        {
            RectTransform inst = Instantiate(dotPrefab, parent);
            pool.Add(inst);
            used += 1;
            return inst;
        }
    }

    private void SetDotSize(RectTransform rt, float size)
    {
        rt.sizeDelta = new Vector2(size, size);
    }
}
