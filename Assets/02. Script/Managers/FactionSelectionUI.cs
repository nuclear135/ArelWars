using UnityEngine;
using UnityEngine.SceneManagement;
using ArelWars.Units;

// 메인 씬에서 종족 버튼이 이 스크립트의 메서드를 호출하게 연결
public class FactionSelectionUI : MonoBehaviour
{
    public void PickHuman()
    {
        Pick(Faction.Human);
    }

    public void PickOrc()
    {
        Pick(Faction.Orc);
    }

    public void PickElf()
    {
        Pick(Faction.Elf);
    }

    // 종족을 적용하고 Game Scene 씬으로 전환
    private void Pick(Faction playerChoice)
    {
        TeamService.ApplySelection(playerChoice);
        SceneManager.LoadScene("Game Scene");
    }
}
