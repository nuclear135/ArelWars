using UnityEngine;
using UnityEngine.SceneManagement;

// - �̱���ó�� �����ϴ� ���� �ν��Ͻ� ����(GameManager.I)
// - Ÿ�� �ı� �� ��/�� ����, ����� ���
public class GameManager : MonoBehaviour
{
    public static GameManager I;   // ���� ���ٿ�(������ ���� ����)

    public Transform playerTower;   // �÷��̾� Ÿ��(������ �Ҵ�)
    public Transform enemyTower;    // �� Ÿ��(������ �Ҵ�)     

    void Awake()
    {
        I = this;
        Application.targetFrameRate = 60; // ������ ����(����� ���)
    }

    // Ÿ�� �ı� �� ȣ���
    public void OnTowerDestroyed(GameObject who)
    {
        bool playerLose = (who == playerTower);
        Debug.Log(playerLose ? "[Game] �й�" : "[Game] �¸�");
        Time.timeScale = 0f; // �Ͻ� ����(����/��� UI �غ�)

        // TODO: �¸�/�й� UI�� ���� ��ư���� Restart() ����
    }

    // ���� �����
    public void Restart()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
