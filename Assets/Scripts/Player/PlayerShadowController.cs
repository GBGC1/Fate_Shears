using UnityEngine;
using UnityEngine.InputSystem; // Input System 사용을 위해 추가

/// <summary>
/// 플레이어의 그림자 오브젝트와 그림자 모드(C키)를 제어합니다.
/// 점프 시 그림자를 숨기고, C키로 그림자 모드를 토글합니다.
/// PlayerInput 이벤트를 구독하고 PlayerLocomotion의 Grounded 상태를 참조합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(PlayerLocomotion), typeof(PlayerInput))]
public class PlayerShadowController : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [Tooltip("플레이어의 자식 오브젝트인 Shadow를 연결해주세요.")]
    [SerializeField] private GameObject shadowObject;

    private SpriteRenderer playerSpriteRenderer;
    private PlayerLocomotion playerLocomotion;
    private PlayerInput playerInput; // PlayerInput 참조 추가

    private Color originalPlayerColor;
    private bool isShadowModeActive = false; // C키로 활성화되는 그림자 모드 상태

    private void Awake()
    {
        // 필요한 컴포넌트들을 가져옵니다.
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerInput = GetComponent<PlayerInput>(); // PlayerInput 컴포넌트 가져오기

        if (shadowObject == null)
        {
            Debug.LogError("Shadow 오브젝트가 연결되지 않았습니다! 인스펙터 창에서 연결해주세요.", this);
            enabled = false;
            return;
        }
        if (playerInput == null) // PlayerInput 확인 추가
        {
             Debug.LogError("PlayerInput 컴포넌트를 찾을 수 없습니다!", this);
            enabled = false;
            return;
        }

        // 플레이어의 원래 색상을 저장해둡니다.
        originalPlayerColor = playerSpriteRenderer.color;

        // PlayerInput의 그림자 토글 이벤트 구독
        playerInput.OnToggleShadowEvent += HandleToggleShadow;
    }

     private void OnDestroy()
    {
        // 이벤트 구독 해지
        if (playerInput != null)
        {
            playerInput.OnToggleShadowEvent -= HandleToggleShadow;
        }
    }


    private void Update() // FixedUpdate 대신 Update 사용 권장 (매 프레임 시각적 업데이트)
    {
        // 점프 시 그림자 처리
        HandleShadowOnJump();
    }

    /// <summary>
    /// PlayerInput의 OnToggleShadowEvent 신호를 받아 그림자 모드를 토글합니다.
    /// </summary>
    private void HandleToggleShadow()
    {
        isShadowModeActive = !isShadowModeActive; // 그림자 모드 상태를 뒤집습니다.
        UpdateShadowVisuals();
    }

    /// <summary>
    /// 그림자 모드 상태에 따라 플레이어와 그림자의 모습을 업데이트합니다.
    /// </summary>
    private void UpdateShadowVisuals()
    {
        if (isShadowModeActive)
        {
            // 그림자 모드 활성화: 그림자 끄고, 플레이어 검게
            shadowObject.SetActive(false);
            playerSpriteRenderer.color = Color.black;
        }
        else
        {
            // 그림자 모드 비활성화: 플레이어 색상 원래대로, 그림자는 지면 상태에 따라 결정
            playerSpriteRenderer.color = originalPlayerColor;
            HandleShadowOnJump(); // 점프 상태에 맞게 그림자를 즉시 업데이트
        }
    }

    /// <summary>
    /// 플레이어의 지면 상태에 따라 그림자를 켜고 끕니다.
    /// (그림자 모드가 아닐 때만 작동)
    /// </summary>
    private void HandleShadowOnJump()
    {
        // C키로 그림자 모드가 켜져있다면, 이 로직은 무시됩니다.
        if (isShadowModeActive) return;

        // PlayerLocomotion의 Grounded 프로퍼티를 사용하여 지면 상태를 확인합니다.
        if (playerLocomotion.Grounded)
        {
            shadowObject.SetActive(true); // 땅에 있으면 그림자 켜기
        }
        else
        {
            shadowObject.SetActive(false); // 공중에 있으면 그림자 끄기
        }
    }
}