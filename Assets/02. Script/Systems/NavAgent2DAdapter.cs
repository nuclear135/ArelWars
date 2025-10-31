using UnityEngine;
using UnityEngine.AI;

// ============================================================
// NavAgent2DAdapter (ArelWars.Systems)
// ------------------------------------------------------------
// - NavMeshAgent(3D)의 desiredVelocity(XZ)를 2D(XY)로 투영하여
//   Rigidbody2D.velocity로 이동을 수행한다.
// ============================================================
namespace ArelWars.Systems
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgent2DAdapter : MonoBehaviour
    {
        [Header("이동 파라미터")]
        public float maxSpeed     = 2.0f;
        public float acceleration = 8f;

        Rigidbody2D  rb;
        NavMeshAgent agent;
        Vector2      currentVel;

        void Awake()
        {
            rb    = GetComponent<Rigidbody2D>();
            agent = GetComponent<NavMeshAgent>();

            agent.updatePosition = false;
            agent.updateRotation = false;

            var p = transform.position;
            transform.position = new Vector3(p.x, p.y, 0f); // 2D: Z=0 고정
        }

        void FixedUpdate()
        {
            Vector3 desired   = agent.desiredVelocity;        // XZ 기준
            Vector2 desired2D = new Vector2(desired.x, desired.z); // XY 투영

            Vector2 targetVel = Vector2.ClampMagnitude(desired2D, maxSpeed);
            currentVel = Vector2.MoveTowards(currentVel, targetVel, acceleration * Time.fixedDeltaTime);

            rb.velocity = currentVel;

            // 에이전트 내부 좌표 동기화(XZ 사용)
            agent.nextPosition = new Vector3(transform.position.x, 0f, transform.position.y);
        }

        public void SetDestination(Vector2 worldPosXY)
        {
            agent.SetDestination(new Vector3(worldPosXY.x, 0f, worldPosXY.y));
        }

        public bool Reached(float stopDistance = 0.2f)
        {
            if (!agent.hasPath) return false;
            return agent.remainingDistance <= stopDistance;
        }

        public void Stop()
        {
            agent.ResetPath();
            rb.velocity = Vector2.zero;
            currentVel  = Vector2.zero;
        }

        public void ResumeTo(Vector2 worldPosXY)
        {
            SetDestination(worldPosXY);
        }
    }
}
