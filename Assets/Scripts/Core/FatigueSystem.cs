using UnityEngine;
using System;
using System.Collections.Generic;

// 피로도 구간 정의
public enum FatigueStage
{
    None,            // 0 - 59% (제한 없음)
    MildlyFatigued,  // 60 - 69% (약간 피로함)
    Fatigued,        // 70 - 79% (피로함)
    HeavilyFatigued, // 80 - 89% (많이 피로함)
    Exhausted        // 90 - 100% (탈진)
}

public class FatigueSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager playerStats;
    [SerializeField] private PlayerLocomotion locomotion;
    [SerializeField] private FatigueEffectVisual visionController;   // 시야 축소/차단 역할
    [SerializeField] private StatusEffectManager statusManager;

    [Header("Current Fatigue Status")]
    [Range(0f, 100f)]
    [SerializeField] private float currentFatigue = 0f;
    [SerializeField] private FatigueStage currentStage = FatigueStage.None;

    [Header("Fatigue Settings")]
    [SerializeField] private float fatigueIncreaseRate = 0.5f;  // 전투/이동 중 초당 피로도 증가율
    [SerializeField] private float restRecoveryAmount = 50f;   // 휴식 시 피로도 회복량
    // 피로도 100이 되어 그림자 상태로 전환 후 해제 시 피로도 회복량
    [SerializeField] private float unshadowRecoveryAmount = 10f;

    private bool isExhaustedTriggered = false;
    private bool isExhaustedFinished = false;

    #region Properties
    public bool IsExhaustedTriggered
    {
        get => isExhaustedTriggered;
        set => isExhaustedTriggered = value;
    }
    public bool IsExhaustedFinished
    {
        get => isExhaustedFinished;
        set => isExhaustedFinished = value;
    }
    #endregion

    private void Awake()
    {
        // 레퍼런스 체크
        if (playerStats == null || locomotion == null || visionController == null)
        {
            Debug.LogError("필수 컴포넌트를 찾지 못했습니다.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // 피로도 100%가 된 후에는 시스템 정지 
        if (isExhaustedTriggered) return;

        // 움직이는 중인 경우 피로도 증가
        if (locomotion.IsMoving || locomotion.IsDashing || locomotion.IsJumping)
        {
            AddFatigue(fatigueIncreaseRate * Time.deltaTime);
        }

        // 피로도 구간 확인
        CheckFatigueStage();
    }
    #region Fatigue Increase/Decrease Methods
    // 이동/대시 시 피로도 증가
    public void AddFatigue(float amount)
    {
        // 피로도 범위 0~100으로 제한
        currentFatigue = Mathf.Clamp(currentFatigue + amount, 0f, 100f);
    }

    // 휴식 장소에서 피로도 회복
    public void RestAtSafeZone()
    {
        // 중독 시 휴식 불가 상태로 무시
        if (!statusManager.CanRest) return;

        // 휴식 시 피로도 회복량만큼 피로도 감소
        AddFatigue(-restRecoveryAmount);

        Debug.Log("휴식 장소에 피로도 회복 (-50)");

        // 휴식 시 탈진 상태 플래그 초기화
        isExhaustedTriggered = false;
        isExhaustedFinished = false;

        // 피로도 구간 확인하여 패널티 해제
        CheckFatigueStage();
    }

    // 피로도에 의한 그림자 전환 후 해제 시 피로도 소량 회복
    private void ApplyUnshadowRecovery()
    {
        // 그림자 해제 시 피로도 회복량만큼 피로도 감소
        AddFatigue(-unshadowRecoveryAmount);

        Debug.Log("그림자 해제 : 피로도 소량 회복");

        // 그림자 해제 시 탈진 상태 플래그 초기화
        isExhaustedTriggered = false;
        isExhaustedFinished = false;

        // 피로도 구간 재확인 (탈진(90%) 상태 유지)
        CheckFatigueStage();
    }
    #endregion

    // 현재 피로도 구간 확인 및 전환
    private void CheckFatigueStage()
    {
        // 현재 피로도 수치에 따른 새로운 피로도 구간 결정
        FatigueStage newStage;
        if (currentFatigue >= 90f) newStage = FatigueStage.Exhausted;
        else if (currentFatigue >= 80f) newStage = FatigueStage.HeavilyFatigued;
        else if (currentFatigue >= 70f) newStage = FatigueStage.Fatigued;
        else if (currentFatigue >= 60f) newStage = FatigueStage.MildlyFatigued;
        else newStage = FatigueStage.None;

        // 피로도 구간이 변경된 경우 피로도 패널티 적용
        if (newStage != currentStage)
        {
            ApplyFatiguePenalty(newStage);
            currentStage = newStage;
        }

        // 현재 피로도가 100 이상이며 탈진 상태 플래그가 모두 false인 경우
        if (currentFatigue >= 100f && !isExhaustedTriggered && !isExhaustedFinished)
        {
            isExhaustedTriggered = true;
            isExhaustedFinished = false;

            Debug.Log("피로도 100% 도달");

            // 3초간 시야 차단 활성화
            visionController.ActivateVisionObstruction();

            // 그림자 전환 (차후 추가)
            // 그림자 해제 시 피로도 소량 회복 (완료)
        }
    }

    private void ApplyFatiguePenalty(FatigueStage stage)
    {
        // Multiplier 기본값으로 리셋
        locomotion.MoveSpeedMultiplier = 1f;
        playerStats.StaminaMultiplier = 1f;

        // 시야 축소 효과는 기본적으로 비활성화
        bool isVisionShrinkActive = false;

        // 피로도 구간별 패널티 설정
        switch (stage)
        {
            case FatigueStage.MildlyFatigued:
                // 이동 속도 소폭 감소 (-10%)
                locomotion.MoveSpeedMultiplier = 0.90f;
                break;
            case FatigueStage.Fatigued:
                // 이동 속도 감소 (-20%)
                locomotion.MoveSpeedMultiplier = 0.80f;
                break;
            case FatigueStage.HeavilyFatigued:
                // 이동 속도 감소 (-30%), 스태미나 소모율 소폭 증가 (+10%)
                locomotion.MoveSpeedMultiplier = 0.70f;
                playerStats.StaminaMultiplier = 1.10f;
                break;
            case FatigueStage.Exhausted:
                // 이동 속도 대폭 감소 (-50%), 스태미나 소모율 증가 (+25%), 시야 축소
                locomotion.MoveSpeedMultiplier = 0.50f;
                playerStats.StaminaMultiplier = 1.25f;
                isVisionShrinkActive = true;    // 시야 축소 플래그 true로 설정
                break;
            case FatigueStage.None:
                locomotion.MoveSpeedMultiplier = 1f;
                playerStats.StaminaMultiplier = 1f;
                break;
        }
        
        // 시야 축소 플래그에 따라 시야 축소 효과 적용/해제
        if (isVisionShrinkActive)
        {
            // 피로도 90% 이상인 경우 시야 축소 활성화
            visionController.ActivateVisionShrink();
        }
        else
        {
            // 피로도 90% 미만인 경우 시야 축소 해제
            visionController.RemoveVisionShrink();
            isVisionShrinkActive = false;
        }
    }
}
