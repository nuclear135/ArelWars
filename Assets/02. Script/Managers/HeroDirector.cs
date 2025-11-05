using UnityEngine;
using ArelWars.Units;

// 팀당 1명의 영웅을 제어
// Pad에서 대기 → 출전(라인 스폰위치로 텔레포트) → 복귀(즉시 Pad로 텔레포트)
// 복귀 후 쿨다운 동안 Pad에서 초당 회복(풀피 가능)
// 사망 시 일정 시간 뒤 Pad에서 부활
public class HeroDirector : MonoBehaviour
{
    [Header("소속/기본")]
    public bool isPlayer = true;

    [Header("참조")]
    public Transform heroPad;     // 타워 위 대기 지점
    public Spawner upSpawner;
    public Spawner downSpawner;

    [Header("영웅 프리팹 (비워두면 종족으로 자동 선택)")]
    [SerializeField] private GameObject heroPrefab;

    [Header("공통 수치")]
    [SerializeField] private float cooldownSeconds = 12f;
    [SerializeField] private float respawnSeconds = 30f;
    [SerializeField] private float healPerSecondAtPad = 0.1f; // MaxHp의 비율로 회복

    private enum State
    {
        IdleAtPad,      // Pad에서 대기(출전 가능)
        Deployed,       // 출전 중
        CooldownAtPad,  // 복귀 후 쿨다운 + 회복
        Dead            // 사망 후 부활 대기
    }

    private State state = State.IdleAtPad;

    private GameObject hero;
    private UnitController2D uc;
    private float timer;
    private Line lastLine = Line.Up; // 마지막으로 선택된 라인 (UI에서 설정)

    private void Start()
    {
        // 영웅 프리팹을 지정하지 않았으면 종족으로 자동 선택
        if (heroPrefab == null)
        {
            Faction f;
            if (isPlayer)
            {
                f = TeamService.PlayerFaction;
            }
            else
            {
                f = TeamService.EnemyFaction;
            }

            heroPrefab = FactionAssets.I.GetHeroPrefab(f);
        }

        SpawnAtPad();
        SetState(State.IdleAtPad);
    }

    private void Update()
    {
        if (state == State.CooldownAtPad)
        {
            if (uc != null)
            {
                // 초당 회복 (MaxHp 비율)
                uc.Heal(uc.GetMaxHp() * healPerSecondAtPad * Time.deltaTime);
            }

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SetState(State.IdleAtPad);
            }
        }
        else if (state == State.Dead)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SpawnAtPad();
                SetState(State.IdleAtPad);
            }
        }
    }

    // UI에서 ↑/↓ 버튼으로 라인을 선택할 때 호출
    public void SetLine(Line line)
    {
        lastLine = line;
    }

    // UI에서 영웅 버튼을 눌렀을 때 호출
    // - 대기 중이면 출전
    // - 출전 중이면 즉시 복귀
    public void ToggleDeployOrRecall()
    {
        if (state == State.IdleAtPad)
        {
            Deploy(lastLine);
        }
        else if (state == State.Deployed)
        {
            RecallImmediate();
        }
    }

    // 선택된 라인의 스폰 위치로 영웅을 이동시켜 출전
    private void Deploy(Line line)
    {
        if (uc == null)
        {
            return;
        }

        Spawner sp;
        if (line == Line.Up)
        {
            sp = upSpawner;
        }
        else
        {
            sp = downSpawner;
        }

        if (sp != null && sp.spawnPoint != null)
        {
            hero.transform.position = sp.spawnPoint.position;
        }

        hero.layer = LayerUtil.GetSpawnLayer(isPlayer, true);

        SetState(State.Deployed);
    }

    // 즉시 Pad로 복귀시키고 쿨다운 시작
    private void RecallImmediate()
    {
        if (uc == null || heroPad == null)
        {
            return;
        }

        hero.transform.position = heroPad.position;
        timer = cooldownSeconds;

        SetState(State.CooldownAtPad);
    }

    // 영웅이 죽었을 때 호출 (애니메이션 이벤트나 Health 훅 등으로 연결 가능)
    public void OnHeroKilled()
    {
        if (hero != null)
        {
            Destroy(hero);
        }

        uc = null;
        hero = null;

        timer = respawnSeconds;

        SetState(State.Dead);
    }

    // Pad 위치에 영웅을 소환
    private void SpawnAtPad()
    {
        if (heroPrefab == null || heroPad == null)
        {
            Debug.LogWarning("[HeroDirector] 영웅 프리팹 또는 Pad가 비어 있습니다.");
            return;
        }

        hero = Instantiate(heroPrefab, heroPad.position, Quaternion.identity);

        hero.tag = TeamService.GetSpawnTag(isPlayer);
        hero.layer = LayerUtil.GetSpawnLayer(isPlayer, true);

        uc = hero.GetComponent<UnitController2D>();
        if (uc != null)
        {
            uc.isPlayer = isPlayer;
            uc.line = lastLine;
            uc.isHero = true;
        }
    }

    private void SetState(State s)
    {
        state = s;
        // TODO: 버튼 인터랙션/쿨다운 표시 등 UI 갱신 로직 추가 가능
    }

    public bool CanDeployNow()
    {
        if (state == State.IdleAtPad)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
