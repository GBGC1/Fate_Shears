using UnityEngine;
using System;
using System.Collections.Generic;

// 보정치 적용 요인(Source) 자격을 부여하는 인터페이스 정의
public interface IStatSource
{
    // Modifier를 제거할 때 사용되며 장비/버프의 고유 ID를 사용
    int SourceID { get; }
}

// 보정치 적용 타입 정의
public enum StatModType
{
    Additive,           // 기본 스탯에 증감하는 값 (ex. +10 공격력)
    Multiplicative      // 합산된 값에 곱하는 비율 (ex. +10% 공격력)
}

// 스탯 보정치
public class StatModifier
{
    public readonly float Value;
    public readonly StatModType Type;
    public readonly IStatSource Source;     // 보정치 적용 요인 (장비, 버프)
    public readonly int SourceID;           // 보정치 제거를 위한 고유 ID

    public StatModifier(float value, StatModType type, IStatSource source)
    {
        Value = value;
        Type = type;
        Source = source;
        SourceID = source.SourceID;     // IStatSource로부터 ID 추출하여 저장
    }
}

/// <summary>
/// 캐릭터의 실시간 스탯을 관리하고 보정치를 적용하는 클래스
/// 스탯 계산, 스탯 변화 이벤트, 사망 처리를 담당
/// </summary>
public class StatManager : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private StatDataSO statDataSO;

    // 현재 스탯
    private float currentHP;
    private float currentStamina;

    // 실시간 기본 스탯 딕셔너리 (<= UpgradeStat에 의해서만 영구적으로 변경)
    private Dictionary<StatType, float> stats = new();

    // 스탯 보정치 딕셔너리 (<= 임시값 저장)
    private Dictionary<StatType, List<StatModifier>> modifiers = new();

    // 최종 스탯 캐시 딕셔너리
    private Dictionary<StatType, float> finalStats = new();

    private bool isDead = false;

    #region Stat Event Actions
    // 스탯 변경 이벤트 선언 (형태 : <sender, currentValue, maxValue>)
    public event Action<StatManager, float, float> OnHPChanged;
    public event Action<StatManager, float, float> OnStaminaChanged;
    public event Action<StatManager, StatType, float> OnFinalStatChanged;

    // PlayerDamageStateManager에게 피격 반응을 요청하는 이벤트
    public event Action<StatManager> OnHurt;
    public event Action OnDeath;
    #endregion

    #region Stat Properties
    // 최종 스탯 프로퍼티 (스탯 변경 시점에만 계산하여 캐시를 저장)
    public float MaxHP => finalStats.GetValueOrDefault(StatType.MaxHP, 0f);
    public float MaxStamina => finalStats.GetValueOrDefault(StatType.MaxStamina, 0f);
    public float AttackPower => finalStats.GetValueOrDefault(StatType.AttackPower, 0f);
    public float DefensePower => finalStats.GetValueOrDefault(StatType.DefensePower, 0f);
    public float MoveSpeed => finalStats.GetValueOrDefault(StatType.MoveSpeed, 0f);

    public float CurrentHP => currentHP;
    public float CurrentStamina => currentStamina;
    public float StaminaMultiplier { get; set; } = 1f;
    public bool IsDead => isDead;
    #endregion

    private void Awake()
    {
        // StatType enum에 정의된 모든 스탯을 기본 스탯 딕셔너리에 초기화
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            stats[statType] = statDataSO.GetInitialStat(statType);
            modifiers[statType] = new List<StatModifier>();

            // 초기 최종 스탯 계산 및 캐싱
            UpdateFinalStat(statType);
        }

        // 현재 HP/스테미나 초기화
        currentHP = MaxHP;
        currentStamina = MaxStamina;

        // 초기 이벤트 호출
        OnHPChanged?.Invoke(this, currentHP, MaxHP);
        OnStaminaChanged?.Invoke(this, currentStamina, MaxStamina);
    }

    #region Modifier Methods
    // 영구적인 스탯 증가/감소 처리 (<= 능력 포인트 투자)
    public void UpgradeStat(StatType statType, float amount)
    {
        // 기본 스탯 값 변경
        stats[statType] += amount;
        stats[statType] = Mathf.Max(stats[statType], 0);

        // 최종 스탯 업데이트
        UpdateFinalStat(statType);
    }

    // 임시 스탯 보정치 추가 (<= 장비 착용, 버프 적용)
    public void AddModifier(StatType statType, StatModifier modifier)
    {
        modifiers[statType].Add(modifier);
        UpdateFinalStat(statType);
    }

    // 임시 스탯 보정치 제거 (<= 장비 해제, 버프 만료)
    public void RemoveModifier(StatType statType, IStatSource sourceToRemove)
    {
        int idToRemove = sourceToRemove.SourceID;

        // 해당 고유 SourceID를 가진 보정치 제거
        int removed = modifiers[statType].RemoveAll(mod => mod.SourceID == idToRemove);

        if (removed > 0)
        {
            UpdateFinalStat(statType);
        }
    }
    #endregion

    #region Final Stat Methods
    // 최종 스탯 계산 처리
    private float CalculateFinalStat(StatType statType)
    {
        if (!stats.ContainsKey(statType)) return 0f;

        float finalValue = stats[statType];

        float additiveBonus = 0f;
        float multiplicativeBonus = 1f;

        // 보정치 할당
        foreach (var mod in modifiers[statType])
        {
            if (mod.Type == StatModType.Additive)
            {
                additiveBonus += mod.Value;
            }
            else
            {
                multiplicativeBonus += mod.Value;
            }
        }

        // Additive 보정치 합산
        finalValue += additiveBonus;
        finalValue = Mathf.Max(finalValue, 0);

        // Multiplicative 보정치 적용
        finalValue *= multiplicativeBonus;

        // 최종 스탯은 0 미만이 되지 않도록 다시 처리
        return Mathf.Max(finalValue, 0);
    }

    // 최종 스탯 변경 처리 및 캐시 업데이트
    private void UpdateFinalStat(StatType statType)
    {
        float finalValue = CalculateFinalStat(statType);
        finalStats[statType] = finalValue;  // 최종 스탯 캐시 업데이트

        if (statType == StatType.MaxHP)
        {
            // MaxHP가 감소 시 현재 HP가 최대치를 초과하지 않도록 조정
            currentHP = Mathf.Min(currentHP, finalValue);
            OnHPChanged?.Invoke(this, currentHP, finalValue);
        }
        else if (statType == StatType.MaxStamina)
        {
            // MaxStamina 감소 시 현재 스테미나가 최대치를 초과하지 않도록 조정
            currentStamina = Mathf.Min(currentStamina, finalValue);
            OnStaminaChanged?.Invoke(this, currentStamina, finalValue);
        }
        else
        {
            // 기타 공격력, 방어력 등의 최종 스탯 변경 이벤트 추가
            OnFinalStatChanged?.Invoke(this, statType, finalValue);
        }
    }
    #endregion

    #region HP/Stamina Methods
    // 데미지 처리 (HP 감소)
    public void TakeDamage(float amount)
    {
        // 이미 사망한 경우 무시
        if (isDead) return;

        // 방어력 적용 데미지 계산
        float defense = DefensePower;
        float finalDamage = amount * (100f / (100f + defense));

        // 계산된 최종 데미지만큼 HP 감소
        currentHP = Mathf.Max(currentHP - finalDamage, 0);
        OnHPChanged?.Invoke(this, currentHP, MaxHP);

        // HP 감소 시 Damage State Manager에게 피격 반응 요청
        // 사망 상태가 아니면 피격 이벤트 발생
        if (currentHP > 0)
        {
            OnHurt?.Invoke(this);
        }
        // 현재 HP가 0 이하가 된 경우 사망 이벤트 발생
        else
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    // HP 회복 (HP 증가)
    public void HealHP(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, MaxHP);
        OnHPChanged?.Invoke(this, currentHP, MaxHP);
    }

    // 스테미너 소모
    public void UseStamina(float amount)
    {
        float finalAmount = amount * StaminaMultiplier;
        if (currentStamina >= finalAmount)
        {
            currentStamina -= finalAmount;
            OnStaminaChanged?.Invoke(this, currentStamina, MaxStamina);
        }
    }

    // 스테미너 회복
    public void HealStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, MaxStamina);
        OnStaminaChanged?.Invoke(this, currentStamina, MaxStamina);
    }

    /// <summary>
    /// 방어력을 무시하고 HP를 즉시 감소시킵니다. (그림자 모드 변신 비용 등)
    /// 이 함수는 '피격'으로 간주되지 않으므로 OnHurt 이벤트를 호출하지 않습니다.
    /// </summary>
    public void ApplyDirectHPLoss(float amount)
    {
        // 순수 데미지만큼 HP 감소
        currentHP = Mathf.Max(currentHP - amount, 0);
        
        // [중요] UI 컨트롤러에 신호를 보냅니다.
        OnHPChanged?.Invoke(this, currentHP, MaxHP);

        // HP가 0이 되면 사망 이벤트는 동일하게 호출
        if (currentHP <= 0)
        {
            // OnDeath 이벤트는 이미 구독되어 있으므로
            // PlayerDamageStateManager가 사망 처리를 할 것입니다.
            OnDeath?.Invoke();
        }
    }
    #endregion

}