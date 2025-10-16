using UnityEngine;
using System;
using System.Collections;

// 상태이상 타입 정의
public enum StatusEffectType
{
    Bleeding,   // 출혈 (천천히 체력 감소)
    Fracture,   // 골절 (이동속도 대폭 감소, 점프 불가)
    Poisoning,  // 중독 (천천히 체력 감소, 휴식 불가)
    Burn        // 화상 (간헐적 체력 감소)
}

/// <summary>
/// 상태이상(출혈/골절/중독/화상) 효과를 관리하는 클래스
/// 코루틴을 통해 지속적인 상태이상 패널티 부여
/// </summary>
public class StatusEffectManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager playerStat;
    [SerializeField] private PlayerLocomotion locomotion;

    [Header("Current Status Effect Flags")]
    [SerializeField] private bool isBleeding = false;
    [SerializeField] private bool isFractured = false;
    [SerializeField] private bool isPoisoning = false;
    [SerializeField] private bool isBurned = false;

    [Header("Bleeding Settings")]
    [SerializeField] private float bleedingDamage = 0.5f;

    [Header("Fracture Settings")]
    [SerializeField] private float fractureMoveSpeedPenalty = 0.7f;

    [Header("Poisoning Settings")]
    [SerializeField] private float poisoningDamage = 0.5f;
    private bool canRest = true;

    [Header("Burn Settings")]
    [SerializeField] private float burnDamage = 1f;
    [SerializeField] private float burnIntervalMin = 2f;
    [SerializeField] private float burnIntervalMax = 5f;

    // 상태이상 적용/해제 이벤트 (UI, 사운드용)
    public event Action<StatusEffectType> OnStatusEffectApplied;
    public event Action<StatusEffectType> OnStatusEffectHealed;

    public bool IsFractured => isFractured;
    public bool CanRest => canRest;

    #region Status Effect Apply Methods
    public void ApplyBleeding()
    {
        if (!isBleeding)
        {
            isBleeding = true;
            StartCoroutine(BleedingEffect());
            OnStatusEffectApplied?.Invoke(StatusEffectType.Bleeding);
        }
    }

    public void ApplyFracture()
    {
        if (!isFractured)
        {
            isFractured = true;
            locomotion.MoveSpeedMultiplier = fractureMoveSpeedPenalty;
            OnStatusEffectApplied?.Invoke(StatusEffectType.Fracture);
        }
    }

    public void ApplyPoisoning()
    {
        if (!isPoisoning)
        {
            isPoisoning = true;
            canRest = false;
            StartCoroutine(PoisoningEffect());
            OnStatusEffectApplied?.Invoke(StatusEffectType.Poisoning);
        }
    }

    public void ApplyBurn()
    {
        if (!isBurned)
        {
            isBurned = true;
            StartCoroutine(BurnEffect());
            OnStatusEffectApplied?.Invoke(StatusEffectType.Burn);
        }
    }
    #endregion

    #region Status Effect Coroutines
    private IEnumerator BleedingEffect()
    {
        while (isBleeding)
        {
            yield return new WaitForSeconds(1f);
            playerStat.TakeDamage(bleedingDamage);
        }
    }

    private IEnumerator PoisoningEffect()
    {
        while (isPoisoning)
        {
            yield return new WaitForSeconds(1f);
            playerStat.TakeDamage(poisoningDamage);
        }
    }

    private IEnumerator BurnEffect()
    {
        while (isBurned)
        {
            float randomInterval = UnityEngine.Random.Range(burnIntervalMin, burnIntervalMax);
            yield return new WaitForSeconds(randomInterval);
            playerStat.TakeDamage(burnDamage);
        }
    }
    #endregion

    #region Status Effects Remove Methods
    public void HealBleeding()
    {
        if (isBleeding)
        {
            isBleeding = false;
            OnStatusEffectHealed?.Invoke(StatusEffectType.Bleeding);
        }
    }

    public void HealFracture()
    {
        if (isFractured)
        {
            isFractured = false;
            locomotion.MoveSpeedMultiplier = 1f;
            OnStatusEffectHealed?.Invoke(StatusEffectType.Fracture);
        }
    }

    public void HealPoisoning()
    {
        if (isPoisoning)
        {
            isPoisoning = false;
            canRest = true;
            OnStatusEffectHealed?.Invoke(StatusEffectType.Poisoning);
        }
    }

    public void HealBurn()
    {
        if (isBurned)
        {
            isBurned = false;
            OnStatusEffectHealed?.Invoke(StatusEffectType.Burn);
        }
    }
    #endregion

    // 모든 상태이상 효과 치료
    public void HealAllStatusEffects()
    {
        HealBleeding();
        HealFracture();
        HealPoisoning();
        HealBurn();
    }
}   
