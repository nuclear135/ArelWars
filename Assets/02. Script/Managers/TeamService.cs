using UnityEngine;
using ArelWars.Units;

// 메인 씬에서 플레이어가 선택한 종족을 저장하고
// 적 종족은 나머지 중에서 랜덤으로 정해주는 서비스
// 태그 문자열도 여기서 공통으로 사용
public class TeamService : MonoBehaviour
{
    public static Faction PlayerFaction { get; private set; } = Faction.Human;
    public static Faction EnemyFaction { get; private set; } = Faction.Orc;

    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";

    // 메인 씬 버튼에서 이 함수를 호출해 플레이어 종족을 저장
    // 적 종족은 플레이어가 고른 걸 제외하고 랜덤으로 선택
    public static void ApplySelection(Faction playerChoice)
    {
        PlayerFaction = playerChoice;

        Faction[] all = new[] { Faction.Human, Faction.Orc, Faction.Elf };

        int idx = Random.Range(0, all.Length);
        while (all[idx] == playerChoice)
        {
            idx = Random.Range(0, all.Length);
        }

        EnemyFaction = all[idx];

        Debug.Log("[TeamService] Player=" + PlayerFaction + ", Enemy=" + EnemyFaction);
    }

    // isPlayer가 true면 "Player", false면 "Enemy" 태그 반환
    public static string GetSpawnTag(bool isPlayer)
    {
        if (isPlayer)
        {
            return PlayerTag;
        }
        else
        {
            return EnemyTag;
        }
    }
}
