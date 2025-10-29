using UnityEngine;
using UnityEngine.SceneManagement;

// - 싱글톤처럼 접근하는 정적 인스턴스 제공(GameManager.I)
// - 타워 파괴 시 승/패 판정, 재시작 기능
public class GameManager : MonoBehaviour
{
    public static GameManager I;   // 전역 접근용(간단한 샘플 수준)

    public Transform playerTower;   // 플레이어 타워(씬에서 할당)
    public Transform enemyTower;    // 적 타워(씬에서 할당)     

    void Awake()
    {
        I = this;
        Application.targetFrameRate = 60; // 프레임 고정(모바일 대비)
    }

    // 타워 파괴 시 호출됨
    public void OnTowerDestroyed(GameObject who)
    {
        bool playerLose = (who == playerTower);
        Debug.Log(playerLose ? "[Game] 패배" : "[Game] 승리");
        Time.timeScale = 0f; // 일시 정지(연출/결과 UI 준비)

        // TODO: 승리/패배 UI를 띄우고 버튼으로 Restart() 연결
    }

    // 게임 재시작
    public void Restart()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
