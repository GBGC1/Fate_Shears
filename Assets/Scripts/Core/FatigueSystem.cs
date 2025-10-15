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
    //private FatigueEffectVisual visualController; // 시각 효과 담당 스크립트

    [Header("Current Fatigue Status")]
    [Range(0f, 100f)]
    [SerializeField] private float currentFatigue = 0f;
    [SerializeField] private FatigueStage currentStage = FatigueStage.None;

    [Header("Fatigue Rate Settings")]
    [SerializeField] private float fatigueIncreaseRate = 0.5f;  // 전투/이동 중 초당 피로도 증가율

    private bool isExhaustedTriggered = false;

    //
    private void Awake()
    {
        // 레퍼런스 체크
        if (playerStats == null || locomotion == null)
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

    // 이동/대시 시 피로도 증가
    public void AddFatigue(float amount)
    {
        // 피로도 범위 0~100으로 제한
        currentFatigue = Mathf.Clamp(currentFatigue + amount, 0f, 100f);
    }

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

        // 현재 피로도가 100이 된 경우 그림자 전환
        if (currentFatigue >= 100f && !isExhaustedTriggered)
        {
            isExhaustedTriggered = true;
            // 그림자 전환
            Debug.Log("피로도 100% 도달");
        }
    }

    private void ApplyFatiguePenalty(FatigueStage stage)
    {
        // Multiplier 기본값으로 리셋
        locomotion.MoveSpeedMultiplier = 1f;
        playerStats.StaminaMultiplier = 1f;

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
                // 이동 속도 대폭 감소 (-50%), 스태미나 소모율 증가 (+25%), 시야 감소
                locomotion.MoveSpeedMultiplier = 0.50f;
                playerStats.StaminaMultiplier = 1.25f;
                //if (visualController != null) visualController.ActivateVisionShrink(true);
                break;
            case FatigueStage.None:
                locomotion.MoveSpeedMultiplier = 1f;
                playerStats.StaminaMultiplier = 1f;
                break;
        }
    }
}
