using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// 플레이어의 그림자 오브젝트와 그림자 모드(C키)를 제어합니다.
/// C키로 버프/디버프(공격력, 이속, 체력)를 토글하고,
/// 플레이어의 물리 레이어를 변경하여 환경 기믹을 통과(무시)할 수 있는 상태를 제공합니다.
/// StatManager의 보정치 시스템(IStatSource)을 구현하여 작동합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(PlayerLocomotion), typeof(StatManager))]
public class PlayerShadowController : MonoBehaviour, IStatSource
{
    [Header("오브젝트 연결")]
    [Tooltip("플레이어의 자식 오브젝트인 Shadow를 연결해주세요.")]
    [SerializeField] private GameObject shadowObject;

    [Header("섀도우 모드 버프/디버프 수치")]
    [Tooltip("공격력 증가량 (예: 0.2f = 20% 증가)")]
    [SerializeField] private float attackPowerBonus = 0.2f;
    [Tooltip("이동 속도 배율 (예: 1.3f = 30% 증가)")]
    [SerializeField] private float moveSpeedMultiplierBonus = 1.3f;
    
    // [주석 처리됨] MaxHP 디버프 대신 CurrentHP 코스트 소모 방식을 사용합니다.
    // [Tooltip("최대 체력 감소량 (예: -0.5f = 50% 감소)")]
    // [SerializeField] private float maxHpDebuff = -0.5f;

    [Header("레이어 설정 (Layer Settings)")]
    [Tooltip("플레이어의 기본 레이어 이름 (예: Default)")]
    [SerializeField] private string playerLayerName = "Default"; // "Player"에서 "Default"로 변경
    [Tooltip("섀도우 모드일 때 사용할 레이어 이름 (예: PlayerShadow)")]
    [SerializeField] private string playerShadowLayerName = "PlayerShadow";

    // 컴포넌트 참조
    private SpriteRenderer playerSpriteRenderer;
    private PlayerLocomotion playerLocomotion;
    private PlayerInput playerInput;
    private StatManager statManager;

    // 내부 상태 변수
    private Color originalPlayerColor;
    private bool isShadowModeActive = false;

    // 스탯 보정치 객체
    private StatModifier attackBuff;
    // [삭제됨] private StatModifier maxHpDebuffModifier;
    
    // 레이어 인덱스 저장
    private int playerLayer;
    private int playerShadowLayer;

    #region IStatSource implementation
    public int SourceID => GetInstanceID();
    #endregion

    public bool IsShadowModeActive => isShadowModeActive;

    private void Awake()
    {
        // 1. 모든 필수 컴포넌트를 가져옵니다.
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerInput = GetComponent<PlayerInput>();
        statManager = GetComponent<StatManager>();

        // 2. 오브젝트 연결 확인
        if (shadowObject == null)
        {
            Debug.LogError("Shadow 오브젝트가 연결되지 않았습니다! 인스펙터 창에서 연결해주세요.", this);
            enabled = false; return;
        }
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput 컴포넌트를 찾을 수 없습니다!", this);
            enabled = false; return;
        }
        
        // 3. 레이어 이름(string)을 레이어 인덱스(int)로 변환하여 저장
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        playerShadowLayer = LayerMask.NameToLayer(playerShadowLayerName);

        if (playerLayer == -1) // playerLayerName이 "Default"인 경우 0을 반환하므로 -1이 아님.
        {
            // "Default" 레이어는 항상 인덱스 0을 반환하며 -1이 될 수 없습니다. 
            // 혹시 "Default"가 아닌 다른 잘못된 이름이 인스펙터에 입력되었을 경우를 대비한 방어 코드입니다.
            if (playerLayerName != "Default") {
                Debug.LogError($"'{playerLayerName}' 레이어를 찾을 수 없습니다. Project Settings > Tags and Layers에서 레이어 이름을 확인하세요.", this);
                enabled = false; return;
            }
        }
        if (playerShadowLayer == -1)
        {
            Debug.LogError($"'PlayerShadow' 레이어를 찾을 수 없습니다. Project Settings > Tags and Layers에서 '{playerShadowLayerName}' 레이어를 생성했는지 확인하세요.", this);
            enabled = false; return;
        }

        // 4. 플레이어 원래 색상 저장
        originalPlayerColor = playerSpriteRenderer.color;

        // 5. 스탯 보정치 객체 생성
        attackBuff = new StatModifier(attackPowerBonus, StatModType.Multiplicative, this);
        
        // [수정됨] MaxHP 디버프 보정치 생성 로직 삭제
        // maxHpDebuffModifier = new StatModifier(maxHpDebuff, StatModType.Multiplicative, this);

        // 6. PlayerInput의 'c' 키 이벤트 구독
        playerInput.OnToggleShadowEvent += HandleToggleShadow;
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.OnToggleShadowEvent -= HandleToggleShadow;
        }
        if (isShadowModeActive && statManager != null)
        {
            RemoveShadowModeBuffs();
        }
    }

    private void Update()
    {
        HandleShadowOnJump();
    }

    /// <summary>
    /// 'c' 키 입력 신호를 받아 섀도우 모드를 토글합니다.
    /// </summary>
    private void HandleToggleShadow()
    {
        isShadowModeActive = !isShadowModeActive; // 모드 상태 반전

        // --- 레이어 변경 로직 ---
        if (isShadowModeActive)
        {
            gameObject.layer = playerShadowLayer;
        }
        else
        {
            gameObject.layer = playerLayer;
        }
        // -------------------------

        UpdateShadowVisuals();
        UpdateShadowModeStats();
    }

    /// <summary>
    /// 섀도우 모드 상태에 따라 시각적 효과(색상, 그림자)를 업데이트합니다.
    /// </summary>
    private void UpdateShadowVisuals()
    {
        if (isShadowModeActive)
        {
            shadowObject.SetActive(false);
            playerSpriteRenderer.color = Color.black;
        }
        else
        {
            playerSpriteRenderer.color = originalPlayerColor;
            HandleShadowOnJump(); 
        }
    }

    /// <summary>
    /// 섀도우 모드 상태에 따라 버프/디버프를 적용하거나 제거합니다.
    /// </summary>
    private void UpdateShadowModeStats()
    {
        if (isShadowModeActive)
        {
            statManager.AddModifier(StatType.AttackPower, attackBuff);
            
            // [수정됨] MaxHP를 깎는 대신, 현재 HP의 절반을 '비용'으로 지불합니다.
            // (StatManager에 ApplyDirectHPLoss 함수가 추가되어 있어야 합니다)
            float hpCost = statManager.CurrentHP / 2f;
            statManager.ApplyDirectHPLoss(hpCost);

            playerLocomotion.MoveSpeedMultiplier = moveSpeedMultiplierBonus;
            Debug.Log("섀도우 모드 활성화: 기믹 통과, 스탯 변경, HP 코스트 지불");
        }
        else
        {
            RemoveShadowModeBuffs();
            Debug.Log("섀도우 모드 비활성화: 모든 스탯/레이어 원상 복구");
}
    }

    /// <summary>
    /// StatManager와 PlayerLocomotion에서 모든 섀도우 모드 보정치를 제거합니다.
    /// </summary>
    private void RemoveShadowModeBuffs()
    {
        statManager.RemoveModifier(StatType.AttackPower, this);
        
        // [수정됨] MaxHP 보정치 제거 로직 삭제
        // statManager.RemoveModifier(StatType.MaxHP, this);
        
        playerLocomotion.MoveSpeedMultiplier = 1f; 
    }

    /// <summary>
    /// 플레이어의 지면 상태에 따라 시각적 그림자를 켜고 끕니다. (섀도우 모드가 아닐 때만)
    /// </summary>
    private void HandleShadowOnJump()
    {
        if (isShadowModeActive) return;

        if (playerLocomotion.Grounded)
        {
            shadowObject.SetActive(true);
        }
        else
        {
            shadowObject.SetActive(false);
        }
    }
}
