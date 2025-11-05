using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgent2DAdapter : MonoBehaviour
{
    [SerializeField] private float acceleration = 8f;
    public float maxSpeed = 2f;

    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private Vector2 vel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // Agent는 우리가 수동으로 동기화
        agent.updatePosition = false;
        agent.updateRotation = false;

        // Z = 0 고정
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, p.y, 0f);
    }

    private void Start()
    {
        // 스폰 직후 NavMesh로 붙이기 (반경 1.0f 탐색)
        EnsureOnNavMesh(1.0f);
    }

    private void FixedUpdate()
    {
        // NavMesh 위가 아니면 갱신하지 않음(에러 방지)
        if (!agent.enabled)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        // Agent가 원하는 속도(XZ)를 2D(XY)로 변환
        Vector3 desired = agent.desiredVelocity;
        Vector2 target = new Vector2(desired.x, desired.z);

        target = Vector2.ClampMagnitude(target, maxSpeed);
        vel = Vector2.MoveTowards(vel, target, acceleration * Time.fixedDeltaTime);

        rb.velocity = vel;

        // 내부 위치 동기화(XZ ← XY)
        agent.nextPosition = new Vector3(transform.position.x, 0f, transform.position.y);
    }

    // 외부에서 목적지 설정할 때는 이 함수만 쓰도록(가드 포함)
    public void SetDestination(Vector2 xy)
    {
        // NavMesh 밖이면 먼저 붙여보기
        if (!EnsureOnNavMesh(1.5f))
        {
            // 붙이지 못했으면 경고만 찍고 종료 (SetDestination 금지)
            Debug.LogWarning("[Nav2D] SetDestination 취소: 에이전트가 NavMesh 위가 아님 @ " + transform.position);
            return;
        }

        // XY → XZ 변환하여 목적지 전달
        agent.SetDestination(new Vector3(xy.x, 0f, xy.y));
    }

    // 목적지를 다시 달리기 위해 호출하는 래퍼
    public void ResumeTo(Vector2 xy)
    {
        SetDestination(xy);
    }

    // 현재 위치 근처의 NavMesh를 찾아 에이전트를 얹는다
    public bool EnsureOnNavMesh(float maxDistance)
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (agent.isOnNavMesh)
        {
            return true;
        }

        Vector2 xy = (Vector2)transform.position;

        NavMeshHit hit;
        // XY → XZ 로 바꿔서 샘플링
        bool ok = NavMesh.SamplePosition(new Vector3(xy.x, 0f, xy.y), out hit, maxDistance, NavMesh.AllAreas);

        if (ok)
        {
            // Agent를 NavMesh 위치로 워프
            agent.Warp(hit.position);

            // Transform도 XY 좌표로 동기화 (Z=0)
            transform.position = new Vector3(hit.position.x, hit.position.z, 0f);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Stop()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        rb.velocity = Vector2.zero;
        vel = Vector2.zero;
    }

    public bool Reached(float stop = 0.2f)
    {
        if (agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                if (agent.hasPath && agent.remainingDistance <= stop)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
