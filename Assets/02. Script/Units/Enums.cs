namespace ArelWars.Units
{
    // 라인은 위(Up), 아래(Down) 두 줄로 구성
    public enum Line
    {
        Up,
        Down
    }

    // 종족 세 가지
    public enum Faction
    {
        Human,
        Orc,
        Elf
    }

    // 유닛 타입 세 가지 (종족과 무관하게 공통 타입)
    public enum UnitType
    {
        Warrior,
        Shielder,
        Archer
    }
}
