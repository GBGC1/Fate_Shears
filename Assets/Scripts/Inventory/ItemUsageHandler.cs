using UnityEngine;

/// <summary>
/// 아이템 사용 시 효과 처리를 담당하는 클래스
/// HP 회복, 상태 이상 치료 효과 적용
/// </summary>
public class ItemUsageHandler : MonoBehaviour
{
    public static ItemUsageHandler Instance { get; private set; }

    [Header("References")]
    [SerializeField] private StatManager playerStat;
    [SerializeField] private StatusEffectManager status;
    [SerializeField] private PlayerLocomotion locomotion;

    [Header("Settings")]
    [SerializeField] private int healAmount = 20;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (playerStat == null || status == null)
        {
            Debug.LogError("ItemUsageHandler : 필수 컴포넌트가 할당되지 않았습니다.");
        }
    }

    // 아이템 사용 효과 처리
    public void UseItem(ItemDataSO itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("사용하려는 아이템 데이터가 null입니다.");
            return;
        }

        // 아이템 타입에 따라 적절한 사용 효과 함수 적용
        switch (itemData.type)
        {
            case ItemType.Heal_HP:
                UseHealHPItem(itemData);
                break;
            case ItemType.Cure_Bleeding:
                UseCureBleedingItem(itemData);
                break;
            case ItemType.Cure_Fracture:
                UseCureFractureItem(itemData);
                break;
            case ItemType.Cure_Poisoning:
                UseCurePoisoningItem(itemData);
                break;
            case ItemType.Cure_Burn:
                UseCureBurnItem(itemData);
                break;
            case ItemType.Artifact:
                UseArtifact(itemData);
                break;
        }
    }

    #region Use Item Methods
    // HP 회복 아이템 (HP 포션) 사용 처리
    private void UseHealHPItem(ItemDataSO itemData)
    {
        playerStat.HealHP(healAmount);
        Debug.Log($"{itemData.itemName} used. Healed {healAmount} HP.");
    }

    // 출혈 치료 아이템 (붕대) 사용 처리
    private void UseCureBleedingItem(ItemDataSO itemData)
    {
        status.HealBleeding();
        Debug.Log($"{itemData.itemName} used. Cured Bleeding.");
    }

    // 골절 치료 아이템 (부목) 사용 처리
    private void UseCureFractureItem(ItemDataSO itemData)
    {
        status.HealFracture();
        Debug.Log($"{itemData.itemName} used. Cured Fracture.");
    }

    // 중독 치료 아이템 (해독제, 약초) 사용 처리
    private void UseCurePoisoningItem(ItemDataSO itemData)
    {
        status.HealPoisoning();
        Debug.Log($"{itemData.itemName} used. Cured Poisoning.");
    }

    // 화상 치료 아이템 (화상 연고) 사용 처리
    private void UseCureBurnItem(ItemDataSO itemData)
    {
        status.HealBurn();
        Debug.Log($"{itemData.itemName} used. Cured Burn.");
    }

    // 유물 사용 처리 (차후 수정 필요)
    private void UseArtifact(ItemDataSO itemData)
    {
        locomotion.MaxJumpCount += 1;
        Debug.Log($"{itemData.itemName} used. Get 3rd Jump.");
    }
    #endregion
}
