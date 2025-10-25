using UnityEngine;

/// <summary>
/// 환경 요소와의 상호작용을 담당하는 클래스
/// 위험 지형 (용암, 독성 늪 지대), 세이프존 처리를 담당
/// </summary>
public class EnvironmentInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager playerStat;
    [SerializeField] private FatigueSystem fatigue;
    [SerializeField] private StatusEffectManager playerStatus;

    [Header("Hazard Terrain Settings")]
    // 위험 지형에 닿을 경우 입는 데미지
    [SerializeField] private float hazardDamage = 5;
    private const int LAVA_LAYER = 7;   // 용암 지대 레이어 번호
    private const int SWAMP_LAYER = 8;  // 독성 늪 지대 레이어 번호

    private void Start()
    {
        if (playerStat == null || fatigue == null || playerStatus == null)
        {
            Debug.LogError("EnvironmentInteraction.cs : 할당되지 않은 필수 컴포넌트가 있습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 세이프존에 닿을 경우 휴식을 통한 피로도 회복
        if (other.CompareTag("SafeZone"))
        {
            Debug.Log("세이프존 도달!");
            fatigue.RestAtSafeZone();
        }

        // 용암 지대에 닿을 경우 화상 상태로 전환
        if (other.gameObject.layer == LAVA_LAYER)
        {
            playerStatus.ApplyBurn();
            playerStat.TakeDamage(hazardDamage);
        }
        
        // 독성 늪 지대에 닿을 경우 중독 상태로 전환
        if (other.gameObject.layer == SWAMP_LAYER)
        {
            playerStatus.ApplyPoisoning();
            playerStat.TakeDamage(hazardDamage);
        }
    }
}
