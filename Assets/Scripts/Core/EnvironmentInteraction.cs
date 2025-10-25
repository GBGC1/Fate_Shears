using UnityEngine;

/// <summary>
/// 환경 요소와의 상호작용을 담당하는 클래스
/// 위험 지형 (용암, 독성 늪 지대), 세이프존 처리를 담당
/// </summary>
public class EnvironmentInteraction : MonoBehaviour
{
    [SerializeField] private StatManager playerStat;
    [SerializeField] private FatigueSystem fatigue;

    private void Start()
    {
        if (playerStat == null || fatigue == null)
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
    }
}
