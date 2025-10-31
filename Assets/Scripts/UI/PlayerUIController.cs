using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 플레이어의 메인 HUD UI (체력, 피로도)를 관리합니다.
/// </summary>
public class PlayerUIController : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("플레이어의 스탯 정보 (HP)")]
    [SerializeField] private StatManager playerStats;
    
    [Tooltip("플레이어의 피로도 시스템 (Fatigue)")]
    [SerializeField] private FatigueSystem fatigueSystem;

    [Header("UI References")]
    [Tooltip("체력(HP)을 표시할 Fill 이미지 (HpBar)")]
    [SerializeField] private Image healthBar; // 변수명 및 타입 변경

    [Tooltip("피로도(Fatigue)를 표시할 Fill 이미지 (FatigueBar)")]
    [SerializeField] private Image fatigueBar; // 변수명 및 타입 변경

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        InitializeUI();
    }

    // ... (SubscribeToEvents, UnsubscribeFromEvents 함수는 동일) ...
    private void SubscribeToEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHPChanged += HandleHPChanged;
        }
        else
        {
            Debug.LogWarning("PlayerUIController: StatManager가 할당되지 않았습니다.", this);
        }

        if (fatigueSystem != null)
        {
            fatigueSystem.OnFatigueChanged += HandleFatigueChanged;
        }
        else
        {
            Debug.LogWarning("PlayerUIController: FatigueSystem이 할당되지 않았습니다.", this);
        }
    }
    private void UnsubscribeFromEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHPChanged -= HandleHPChanged;
        }
        
        if (fatigueSystem != null)
        {
            fatigueSystem.OnFatigueChanged -= HandleFatigueChanged;
        }
    }

    /// <summary>
    /// UI 요소들의 초기 상태를 강제로 설정합니다.
    /// </summary>
    private void InitializeUI()
    {
        // HP 슬라이더 초기화
        if (playerStats != null)
        {
            HandleHPChanged(playerStats, playerStats.CurrentHP, playerStats.MaxHP);
        }

        // 피로도 슬라이더 초기화
        if (fatigueSystem != null)
        {
            HandleFatigueChanged(fatigueSystem, fatigueSystem.CurrentFatigue, fatigueSystem.MaxFatigue);
        }
    }

    /// <summary>
    /// HP 변경 이벤트(OnHPChanged)를 받아 healthBar의 fillAmount를 업데이트합니다.
    /// </summary>
    private void HandleHPChanged(StatManager sender, float currentValue, float maxValue)
    {
        // [수정됨] healthSlider -> healthBar
        if (healthBar == null) return;

        // maxValue가 0일 경우 0으로 나누기(NaN) 방지
        float fillValue = (maxValue > 0) ? (currentValue / maxValue) : 0f;

        // [수정됨] .value -> .fillAmount
        // fillAmount는 0~1 사이 값이어야 하므로 Clamp01로 안전하게 처리
        healthBar.fillAmount = Mathf.Clamp01(fillValue);
    }

    /// <summary>
    /// 피로도 변경 이벤트(OnFatigueChanged)를 받아 fatigueBar의 fillAmount를 업데이트합니다.
    /// </summary>
    private void HandleFatigueChanged(FatigueSystem sender, float currentValue, float maxValue)
    {
        // [수정됨] fatigueSlider -> fatigueBar
        if (fatigueBar == null) return;

        // maxValue가 0일 경우 0으로 나누기(NaN) 방지
        float fillValue = (maxValue > 0) ? (currentValue / maxValue) : 0f;
        
        // [수정됨] .value -> .fillAmount
        fatigueBar.fillAmount = Mathf.Clamp01(fillValue);
    }
}