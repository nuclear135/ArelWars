using UnityEngine;
using ArelWars.Units;

// 유닛 선택 바 관리
// - 각 칸에 아이콘/이름/클릭 동작을 설정
// - use=false이거나 슬롯이 비어 있으면 EMPTY로 표시
public class UnitBarUI : MonoBehaviour
{
    [System.Serializable]
    public class UnitSlotConfig
    {
        public SlotButton slot;
        public Sprite icon;
        public string displayName = "Unit";
        public bool use = true;
        public UnitType type = UnitType.Warrior;
    }

    [SerializeField] private Spawner playerUp;
    [SerializeField] private Spawner playerDown;

    [SerializeField] private Line currentLine = Line.Up;

    [SerializeField] private UnitSlotConfig[] slots;

    private void Start()
    {
        RefreshAll();
    }

    public void SelectUp()
    {
        currentLine = Line.Up;
    }

    public void SelectDown()
    {
        currentLine = Line.Down;
    }

    public void RefreshAll()
    {
        foreach (UnitSlotConfig cfg in slots)
        {
            if (cfg.slot == null)
            {
                continue;
            }

            if (!cfg.use)
            {
                cfg.slot.SetEmpty();
                continue;
            }

            cfg.slot.SetAssigned(cfg.icon, cfg.displayName, () => Spawn(cfg.type));
        }
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
}
