using UnityEngine;

// 레이어 인덱스 캐시 및 편의 함수들
public static class LayerUtil
{
    // 레이어 인덱스를 미리 캐싱해두면 성능과 가독성에 유리
    public static readonly int L_PlayerUnit = LayerMask.NameToLayer("Player Unit");
    public static readonly int L_PlayerHero = LayerMask.NameToLayer("Player Hero");
    public static readonly int L_PlayerTower = LayerMask.NameToLayer("Player Tower");

    public static readonly int L_EnemyUnit = LayerMask.NameToLayer("Enemy Unit");
    public static readonly int L_EnemyHero = LayerMask.NameToLayer("Enemy Hero");
    public static readonly int L_EnemyTower = LayerMask.NameToLayer("Enemy Tower");

    // 내 진영이 플레이어인지 여부에 따라 공격 대상 레이어 마스크를 생성
    public static int GetAttackMask(bool isPlayerSide)
    {
        if (isPlayerSide)
        {
            return (1 << L_EnemyUnit) | (1 << L_EnemyHero) | (1 << L_EnemyTower);
        }
        else
        {
            return (1 << L_PlayerUnit) | (1 << L_PlayerHero) | (1 << L_PlayerTower);
        }
    }

    // 스폰 시 적용할 레이어를 결정 (유닛/영웅 구분)
    public static int GetSpawnLayer(bool isPlayer, bool isHero)
    {
        if (isPlayer)
        {
            if (isHero)
            {
                return L_PlayerHero;
            }
            else
            {
                return L_PlayerUnit;
            }
        }
        else
        {
            if (isHero)
            {
                return L_EnemyHero;
            }
            else
            {
                return L_EnemyUnit;
            }
        }
    }
}
