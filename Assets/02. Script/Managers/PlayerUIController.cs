using UnityEngine;
using UnityEngine.UI;
using ArelWars.Units;

// ↑/↓ 라인 선택, 유닛 3종 스폰, 영웅 출전/복귀 토글을 담당
public class PlayerUIController : MonoBehaviour
{
    [Header("플레이어 스포너 (Up/Down)")]
    public Spawner playerUp;
    public Spawner playerDown;

    [Header("플레이어 영웅 지휘자")]
    public HeroDirector playerHero;

    [Header("영웅 버튼 (선택)")]
    public Button heroButton;

    private Line currentLine = Line.Up;

    public void SelectUp()
    {
        currentLine = Line.Up;

        if (playerHero != null)
        {
            playerHero.SetLine(currentLine);
        }
    }

    public void SelectDown()
    {
        currentLine = Line.Down;

        if (playerHero != null)
        {
            playerHero.SetLine(currentLine);
        }
    }

    public void SpawnWarrior()
    {
        Spawn(UnitType.Warrior);
    }

    public void SpawnShielder()
    {
        Spawn(UnitType.Shielder);
    }

    public void SpawnArcher()
    {
        Spawn(UnitType.Archer);
    }

    private void Spawn(UnitType t)
    {
        Spawner sp;

        if (currentLine == Line.Up)
        {
            sp = playerUp;
        }
        else
        {
            sp = playerDown;
        }

        if (sp != null)
        {
            sp.SpawnUnit(t);
        }
    }

    public void ToggleHero()
    {
        if (playerHero != null)
        {
            playerHero.ToggleDeployOrRecall();
        }
    }
}
