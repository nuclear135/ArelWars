using UnityEngine;
using ArelWars.Units;

// 종족별 프리팹을 인스펙터에서 연결해두고
// 코드에서는 이 클래스를 통해 가져와서 Instantiate 한다
public class FactionAssets : MonoBehaviour
{
    [System.Serializable]
    public class UnitSet
    {
        public GameObject warrior;
        public GameObject shielder;
        public GameObject archer;
    }

    [Header("Human (Hero: VINCENT)")]
    [SerializeField] private GameObject humanHero;
    [SerializeField] private UnitSet humanUnits;

    [Header("Orc (Hero: HELBA)")]
    [SerializeField] private GameObject orcHero;
    [SerializeField] private UnitSet orcUnits;

    [Header("Elf (Hero: YUNO)")]
    [SerializeField] private GameObject elfHero;
    [SerializeField] private UnitSet elfUnits;

    // 싱글턴처럼 간단히 접근하기 위한 정적 인스턴스
    private static FactionAssets _i;
    public static FactionAssets I
    {
        get { return _i; }
    }

    private void Awake()
    {
        if (_i != null && _i != this)
        {
            Destroy(gameObject);
            return;
        }

        _i = this;
        DontDestroyOnLoad(gameObject);
    }

    // 종족 + 유닛 타입을 입력받아 해당 프리팹을 돌려준다
    public GameObject GetUnitPrefab(Faction f, UnitType t)
    {
        UnitSet set;

        if (f == Faction.Human)
        {
            set = humanUnits;
        }
        else if (f == Faction.Orc)
        {
            set = orcUnits;
        }
        else
        {
            set = elfUnits;
        }

        if (t == UnitType.Warrior)
        {
            return set.warrior;
        }
        else if (t == UnitType.Shielder)
        {
            return set.shielder;
        }
        else
        {
            return set.archer;
        }
    }

    // 종족에 맞는 영웅 프리팹 반환
    public GameObject GetHeroPrefab(Faction f)
    {
        if (f == Faction.Human)
        {
            return humanHero;
        }
        else if (f == Faction.Orc)
        {
            return orcHero;
        }
        else
        {
            return elfHero;
        }
    }
}
