using UnityEngine;
using ArelWars.Units;

// 팀 × 라인 스포너
// 플레이어/적 각각 Up/Down에 하나씩 배치하여 총 4개 사용
public class Spawner : MonoBehaviour
{
    [Header("소속/라인")]
    public bool isPlayerSpawner = true;
    public Line line = Line.Up;

    [Header("스폰 위치")]
    public Transform spawnPoint;

    // 버튼이나 AI에서 호출하여 유닛을 생성
    public void SpawnUnit(UnitType type)
    {
        if (spawnPoint == null)
        {
            return;
        }

        Faction faction;
        if (isPlayerSpawner)
        {
            faction = TeamService.PlayerFaction;
        }
        else
        {
            faction = TeamService.EnemyFaction;
        }

        GameObject prefab = FactionAssets.I.GetUnitPrefab(faction, type);

        if (prefab == null)
        {
            Debug.LogWarning("[Spawner] 프리팹이 비어 있습니다.");
            return;
        }

        GameObject go = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // 태그/레이어/초기 설정
        go.tag = TeamService.GetSpawnTag(isPlayerSpawner);
        go.layer = LayerUtil.GetSpawnLayer(isPlayerSpawner, false);

        UnitController2D uc = go.GetComponent<UnitController2D>();
        if (uc != null)
        {
            uc.isPlayer = isPlayerSpawner;
            uc.line = line;
            uc.isHero = false;
        }
    }
}
