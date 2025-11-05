using UnityEngine;
using UnityEngine.SceneManagement;
using ArelWars.Units;

// 타워 참조, 승패 판정, 재시작을 담당하는 간단한 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("타워 (팀당 1개)")]
    public Transform playerTower;
    public Transform enemyTower;

    [Header("영웅 대기 Pad (타워 위)")]
    public Transform playerHeroPad;
    public Transform enemyHeroPad;

    private void Awake()
    {
        I = this;
        Application.targetFrameRate = 60;
    }

    // 종족이 결정된 뒤, 타워에 태그 적용 (Player / Enemy)
    public void ApplyTeamTagsAfterSelection()
    {
        if (playerTower != null)
        {
            playerTower.tag = TeamService.PlayerTag;
        }

        if (enemyTower != null)
        {
            enemyTower.tag = TeamService.EnemyTag;
        }
    }

    // 특정 라인/진영에서 적 타워의 위치를 반환 (현재는 라인과 무관)
    public Vector2 GetEnemyTowerPosition(Line line, bool isPlayer)
    {
        Transform t = isPlayer ? enemyTower : playerTower;

        if (t != null)
        {
            return (Vector2)t.position;
        }
        else
        {
            return Vector2.zero;
        }
    }

    // 타워가 파괴되면 호출되어 승패를 출력하고 일시정지
    public void OnTowerDestroyed(GameObject who)
    {
        bool lose = false;

        if (playerTower != null && who.transform == playerTower)
        {
            lose = true;
        }

        if (lose)
        {
            Debug.Log("[Game] 패배");
        }
        else
        {
            Debug.Log("[Game] 승리");
        }

        Time.timeScale = 0f;
    }

    // 재시작
    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
