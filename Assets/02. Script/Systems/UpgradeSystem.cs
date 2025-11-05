using UnityEngine;

// 간단한 전역 강화 시스템
// - 레벨 업 시 공격/체력/이속 배율이 증가
// - 코스트는 지수적으로 상승 (샘플 값, 필요 시 조절)
public class UpgradeSystem : MonoBehaviour
{
    [Header("레벨 및 배율")]
    [SerializeField] private int level = 0;
    [SerializeField] private float atkPerLevel = 0.1f;  // 레벨당 +10%
    [SerializeField] private float hpPerLevel = 0.1f;
    [SerializeField] private float spdPerLevel = 0.05f;

    [Header("코스트")]
    [SerializeField] private int baseCost = 50;
    [SerializeField] private float costScale = 1.4f;

    public int Level
    {
        get { return level; }
    }

    public float AtkMul
    {
        get { return 1f + level * atkPerLevel; }
    }

    public float HpMul
    {
        get { return 1f + level * hpPerLevel; }
    }

    public float SpdMul
    {
        get { return 1f + level * spdPerLevel; }
    }

    public int NextCost
    {
        get { return Mathf.RoundToInt(baseCost * Mathf.Pow(costScale, level)); }
    }

    // 예시: 골드가 충분하면 레벨을 올리고 true 반환
    public bool TryUpgrade(ref int gold)
    {
        int cost = NextCost;

        if (gold < cost)
        {
            return false;
        }

        gold -= cost;
        level += 1;
        return true;
    }
}
