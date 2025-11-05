using UnityEngine;

// 모든 피격 가능한 오브젝트가 붙이는 간단한 체력 스크립트
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 200f;

    private float hp;

    private void Awake()
    {
        hp = maxHp;
    }

    // 데미지를 적용
    public void Damage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0f)
        {
            Die();
        }
    }

    // 파괴 처리
    private void Die()
    {
        // 타워가 죽은 경우에는 게임 매니저에 알린다
        if (gameObject.layer == LayerUtil.L_PlayerTower || gameObject.layer == LayerUtil.L_EnemyTower)
        {
            GameManager.I.OnTowerDestroyed(gameObject);
        }

        Destroy(gameObject);
    }

    // 초당 회복 등에 사용
    public void Heal(float amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);
    }

    // UI 게이지 등에 사용
    public float Hp01
    {
        get
        {
            return Mathf.Clamp01(hp / Mathf.Max(1f, maxHp));
        }
    }

    public float MaxHp
    {
        get
        {
            return maxHp;
        }
    }
}
