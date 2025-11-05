using UnityEngine;
using ArelWars.Units;

// 유닛/영웅 공용 컨트롤러
// - NavAgent2DAdapter로 목적지(적 타워)까지 이동
// - 정면 레이캐스트로 공격
// - 애니메이션 파라미터: Speed(float), Attack(trigger), Die(trigger)
[RequireComponent(typeof(NavAgent2DAdapter))]
public class UnitController2D : MonoBehaviour
{
    [Header("소속/라인")]
    public bool isPlayer = true;     // 플레이어 진영인지
    public Line line = Line.Up;      // Up / Down 라인
    public bool isHero = false;      // 영웅 여부

    [Header("스탯 (인스펙터에서 조절)")]
    [SerializeField] private float maxHp = 120f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attack = 12f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackInterval = 0.8f;

    [Header("공격 위치 기준")]
    [SerializeField] private Transform muzzle; // 총구/앞머리 같은 기준점 (없으면 transform)

    [Header("애니메이터(선택)")]
    [SerializeField] private Animator animator;

    private float hp;
    private float atkTimer;
    private int rayMask;
    private Vector2 enemyTowerPos;
    private NavAgent2DAdapter agent;

    private void Start()
    {
        hp = maxHp;

        agent = GetComponent<NavAgent2DAdapter>();
        agent.maxSpeed = moveSpeed;

        // 스프라이트 좌우 반전
        Vector3 s = transform.localScale;
        if (isPlayer)
        {
            transform.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(s.x), s.y, s.z);
        }

        // 공격 마스크
        rayMask = LayerUtil.GetAttackMask(isPlayer);

        // 목적지 계산
        enemyTowerPos = GameManager.I.GetEnemyTowerPosition(line, isPlayer);

        // ★★★ NavMesh에 먼저 올려놓고 → 목적지 설정 (순서 중요)
        agent.EnsureOnNavMesh(1.0f);
        agent.SetDestination(enemyTowerPos);

        // 레이어 지정
        gameObject.layer = LayerUtil.GetSpawnLayer(isPlayer, isHero);
    }

    private void Update()
    {
        // 공격 쿨다운 감소
        atkTimer -= Time.deltaTime;

        // 레이캐스트 시작점과 방향을 계산
        Vector2 origin;
        if (muzzle != null)
        {
            origin = (Vector2)muzzle.position;
        }
        else
        {
            origin = (Vector2)transform.position;
        }

        Vector2 dir;
        if (isPlayer)
        {
            dir = Vector2.right;   // 플레이어는 오른쪽으로 전진
        }
        else
        {
            dir = Vector2.left;    // 적은 왼쪽으로 전진
        }

        // 앞에 공격 대상이 있는지 레이캐스트로 확인
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, attackRange, rayMask);

        if (hit.collider != null)
        {
            // 가까이에 적이 있다 → 이동 정지 후 공격
            agent.Stop();

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }

            if (atkTimer <= 0f)
            {
                atkTimer = attackInterval;

                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                Health h = hit.collider.GetComponent<Health>();
                if (h != null)
                {
                    h.Damage(attack);
                }
            }
        }
        else
        {
            // 앞이 비었다 → 목적지로 계속 이동
            if (!agent.Reached(0.3f))
            {
                agent.ResumeTo(enemyTowerPos);

                if (animator != null)
                {
                    animator.SetFloat("Speed", moveSpeed);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                }
            }
        }
    }

    // 외부에서 피해를 입혔을 때 호출 가능한 함수 (프로젝트 확장용)
    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            Destroy(gameObject, 0.15f);
        }
    }

    public void Heal(float amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);
    }

    public float GetMaxHp()
    {
        return maxHp;
    }
}
