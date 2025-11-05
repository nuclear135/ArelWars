using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // UI 위 클릭 무시용

public class CameraMove : MonoBehaviour
{
    [Header("스텝 이동 설정 (클릭을 뗄 때 적용)")]
    [SerializeField] private float stepDistance = 5f;      // 한 번에 이동할 X 거리(월드 단위)
    [SerializeField] private bool smoothStep = true;       // true면 부드럽게 이동(코루틴), false면 즉시 텔레포트
    [SerializeField] private float moveDuration = 0.2f;    // 부드러운 이동 시간(초)
    [SerializeField] private float fixedY = 0f;            // 카메라 Y 고정값

    [Header("클릭 판정 (좌/우 분할 기준)")]
    [Range(0.1f, 0.9f)]
    [SerializeField] private float splitRatio = 0.5f;      // 화면의 몇 퍼센트를 기준으로 좌/우를 나눌지 (0.5 = 반반)

    [Header("이동 제한 설정 (양 옆 BoxCollider2D 필요)")]
    [SerializeField] private BoxCollider2D leftBoundary;   // 왼쪽 경계 박스
    [SerializeField] private BoxCollider2D rightBoundary;  // 오른쪽 경계 박스

    [Header("옵션")]
    [SerializeField] private bool lockInsideBounds = true;     // 카메라 화면이 경계 밖을 보이지 않도록 halfWidth 고려
    [SerializeField] private bool ignoreWhenPointerOverUI = true; // UI 위 클릭은 무시

    [Header("내부 상태")]
    private Camera cam;                // 카메라 참조
    private bool isMoving = false;     // 부드러운 이동 중 중복 입력 방지
    private Coroutine moveRoutine = null;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // UI 위에서의 입력은 무시(옵션)
        if (ignoreWhenPointerOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 마우스를 "뗄 때" 방향을 판정하여 스텝 이동 수행
        if (Input.GetMouseButtonUp(0))
        {
            // 이동 중이면 추가 입력은 무시(원하면 큐잉 로직으로 바꿀 수 있음)
            if (isMoving)
            {
                return;
            }

            int dir = GetReleaseDirection(); // -1(왼쪽), +1(오른쪽), 0(중앙)
            if (dir != 0)
            {
                TryStep(dir);
            }
        }
    }

    // 클릭을 뗀 위치가 화면 좌/우 어느 쪽인지 판정.
    // - 반환값: -1(왼쪽), +1(오른쪽), 0(경계선/판정불가)
    private int GetReleaseDirection()
    {
        float screenX = Input.mousePosition.x;
        float cut = Screen.width * splitRatio;

        if (screenX < cut)
        {
            return -1;
        }
        else if (screenX > cut)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // dir 방향으로 stepDistance만큼 이동을 시도하고, 경계를 고려해 목표 X를 계산한 뒤 이동.
    private void TryStep(int dir)
    {
        // 현재 위치
        Vector3 pos = transform.position;

        // 목표 X (기본: 현재 + (방향 * 스텝))
        float targetX = pos.x + dir * stepDistance;

        // 경계가 세팅되어 있다면 클램프 범위 계산
        if (leftBoundary != null && rightBoundary != null)
        {
            float minX, maxX;
            GetClampRange(out minX, out maxX);

            // 스테이지 폭이 카메라보다 좁은 경우: 중앙 고정
            if (minX > maxX)
            {
                targetX = (leftBoundary.bounds.center.x + rightBoundary.bounds.center.x) * 0.5f;
            }
            else
            {
                targetX = Mathf.Clamp(targetX, minX, maxX);
            }
        }

        // 이동 방식: 부드럽게 or 즉시
        if (smoothStep)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            moveRoutine = StartCoroutine(MoveToX(targetX, moveDuration));
        }
        else
        {
            pos.x = targetX;
            pos.y = fixedY; // Y 고정
            transform.position = pos;
        }
    }

    // 카메라의 halfWidth를 고려하여 화면이 경계 밖을 보이지 않도록 X 클램프 범위를 계산.
    private void GetClampRange(out float minX, out float maxX)
    {
        Bounds lb = leftBoundary.bounds;
        Bounds rb = rightBoundary.bounds;

        if (lockInsideBounds && cam != null && cam.orthographic)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            // 왼쪽 경계의 오른쪽 면 기준 +halfWidth, 오른쪽 경계의 왼쪽 면 기준 -halfWidth
            minX = lb.max.x + halfWidth;
            maxX = rb.min.x - halfWidth;
        }
        else
        {
            // 화면 바깥 일부 노출 허용(경계 중앙선 기준)
            minX = lb.center.x;
            maxX = rb.center.x;
        }
    }

    // X축 목표 지점까지 moveDuration 동안 부드럽게 이동(Lerp).
    private IEnumerator MoveToX(float targetX, float duration)
    {
        // 이동 중임을 표시 (다른 입력 무시용)
        isMoving = true;

        // 현재 카메라 위치 저장
        Vector3 start = transform.position;

        // 목표 위치 계산 (Y는 고정, Z는 현재 유지)
        Vector3 end = new Vector3(targetX, fixedY, start.z);

        // 시간 진행 변수 초기화
        float t = 0f;

        // duration이 0 이하일 경우 오류 방지를 위해 최소값 보정
        duration = Mathf.Max(0.0001f, duration);

        // 0~1까지 t를 증가시키면서 부드럽게 이동
        while (t < 1f)
        {
            // t를 시간에 따라 0~1로 선형 증가
            t += Time.deltaTime / duration;

            // start→end로 부드럽게 이동 (SmoothStep으로 감속 효과 추가)
            transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));

            // 한 프레임 대기 (다음 프레임에서 다시 실행)
            yield return null;
        }

        // 이동이 끝나면 정확히 목표 위치에 맞춤
        transform.position = end;

        // 이동 완료 → 상태 초기화
        isMoving = false;
        moveRoutine = null;
    }

    // Scene 뷰에서 경계 시각화(디버그)
    private void OnDrawGizmosSelected()
    {
        // 왼쪽 경계 박스가 존재할 경우
        if (leftBoundary != null)
        {
            // Gizmo 색상을 빨간색으로 지정
            Gizmos.color = Color.red;

            // 왼쪽 경계 박스의 "오른쪽 면" 기준으로 위아래로 긴 세로선 표시
            Gizmos.DrawLine(
                new Vector3(leftBoundary.bounds.max.x, -999f, 0f),
                new Vector3(leftBoundary.bounds.max.x, 999f, 0f)
            );
        }

        // 오른쪽 경계 박스가 존재할 경우
        if (rightBoundary != null)
        {
            // Gizmo 색상을 파란색으로 지정
            Gizmos.color = Color.blue;

            // 오른쪽 경계 박스의 "왼쪽 면" 기준으로 위아래로 긴 세로선 표시
            Gizmos.DrawLine(
                new Vector3(rightBoundary.bounds.min.x, -999f, 0f),
                new Vector3(rightBoundary.bounds.min.x, 999f, 0f)
            );
        }
    }
}
