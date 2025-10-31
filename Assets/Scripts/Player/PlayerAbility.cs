using UnityEngine;
using System;

/// <summary>
/// 플레이어의 성장 능력치(레벨)와 재화를 관리하고,
/// 레벨에 따른 실제 능력치 '값'을 계산하여 제공합니다.
/// </summary>
public class PlayerAbility : MonoBehaviour
{
    [Header("능력치 레벨 (Ability Levels)")]
    [SerializeField] private int might = 1;       // 무력 레벨
    [SerializeField] private int temperance = 1;  // 절제 레벨
    [SerializeField] private int spirit = 1;      // 기백 레벨
    [SerializeField] private int insight = 1;     // 통찰 레벨
    [SerializeField] private int agility = 1;     // 민첩 레벨

    [Header("재화 및 비용")]
    [SerializeField] private int shadowFragments = 20;
    [SerializeField] private int upgradeCost = 1;

    // UI 업데이트를 위한 이벤트. <능력치 타입, 새로운 레벨>
    public event Action<AbilityType, int> OnAbilityUpgraded;
    // 재화 변경 시 UI 업데이트를 위한 이벤트
    public event Action<int> OnShadowFragmentsChanged;

    #region Properties
    // --- 외부에서 '레벨'을 읽기 위한 프로퍼티 ---
    public int Might => might;
    public int Temperance => temperance;
    public int Spirit => spirit;
    public int Insight => insight;
    public int Agility => agility;

    // --- 외부에서 계산된 실제 '값'을 읽기 위한 프로퍼티 ---
    // 이 계산식은 기획에 따라 자유롭게 수정할 수 있습니다.
    public int MightValue => might * 5;         // 예: 무력은 레벨당 5의 공격력
    public int TemperanceValue => temperance * 3; // 예: 절제는 레벨당 3의 방어력
    public int SpiritValue => spirit * 5;       // 예: 기백은 레벨당 5의 특수 공격력
    public int InsightValue => insight * 2;     // 예: 통찰은 레벨당 2%의 치명타 확률
    public int AgilityValue => agility * 2;     // 예: 민첩은 레벨당 2%의 회피율

    public int ShadowFragments => shadowFragments;
    #endregion

    public enum AbilityType
    {
        Might,
        Temperance,
        Spirit,
        Insight,
        Agility
    }

    #region Button Click Handlers
    // 버튼 연결을 위한 public 함수들 (파라미터 없음)
    public void UpgradeMight()     { UpgradeAbility(AbilityType.Might); }
    public void UpgradeTemperance(){ UpgradeAbility(AbilityType.Temperance); }
    public void UpgradeSpirit()    { UpgradeAbility(AbilityType.Spirit); }
    public void UpgradeInsight()   { UpgradeAbility(AbilityType.Insight); }
    public void UpgradeAgility()   { UpgradeAbility(AbilityType.Agility); }
    #endregion

    // 핵심 업그레이드 로직
    private void UpgradeAbility(AbilityType type)
    {
        if (shadowFragments >= upgradeCost)
        {
            shadowFragments -= upgradeCost;
            OnShadowFragmentsChanged?.Invoke(shadowFragments);

            switch (type)
            {
                case AbilityType.Might:
                    might++;
                    OnAbilityUpgraded?.Invoke(type, might);
                    break;
                case AbilityType.Temperance:
                    temperance++;
                    OnAbilityUpgraded?.Invoke(type, temperance);
                    break;
                case AbilityType.Spirit:
                    spirit++;
                    OnAbilityUpgraded?.Invoke(type, spirit);
                    break;
                case AbilityType.Insight:
                    insight++;
                    OnAbilityUpgraded?.Invoke(type, insight);
                    break;
                case AbilityType.Agility:
                    agility++;
                    OnAbilityUpgraded?.Invoke(type, agility);
                    break;
            }
            Debug.Log($"{type} 능력 업그레이드! 현재 그림자 조각: {shadowFragments}");
        }
        else
        {
            Debug.Log("그림자 조각이 부족합니다.");
        }
    }

    // 외부에서 그림자 조각 추가
    public void AddShadowFragments(int amount)
    {
        shadowFragments += amount;
        OnShadowFragmentsChanged?.Invoke(shadowFragments);
        Debug.Log($"그림자 조각 {amount}개 추가! 현재 그림자 조각: {shadowFragments}");
    }
}

