using Weapon;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

/// <summary>
/// 플레이어의 이동, 점프, 대시 입력을 감지하고 상태를 관리하는 클래스  
/// Input System actions를 통해 입력 처리
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private StatManager statManager;
    private UnityEngine.InputSystem.PlayerInput inputSystem;

    private Vector2 moveVector;
    
    // 플레이어 입력 이벤트 선언 (점프/대시)
    public event Action OnJumpEvent;
    public event Action OnDashEvent;
    // 능력치 창 토글을 위한 새 이벤트 추가
    public event Action OnToggleAbilityWindowEvent;
    public event Action OnToggleShadowEvent;
    // 인벤토리 이벤트 선언
    public event Action OnInventoryEvent;

    public event Action OnAttackEvent;
    public event Action<WeaponType> OnChangeFormEvent;
    
    #region Player Input Properties
    public Vector2 MoveVector => moveVector;
    #endregion

    private void Awake()
    {
        statManager = GetComponent<StatManager>();
        inputSystem = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        // 사망 이벤트 구독
        statManager.OnDeath += DisableInput;
    }
    
    private void OnDestroy()
    {
        if (statManager != null)
        {
            statManager.OnDeath -= DisableInput;
        }
    }

    // Move 핸들러: 이동 입력(Vector2) 처리
    void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
    }

    // Jump 핸들러: 점프 버튼(Space) 입력 감지
    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            OnJumpEvent?.Invoke();
        }
    }

    // Dash 핸들러: 대시 버튼(Left Shift) 입력 감지
    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            OnDashEvent?.Invoke();
        }
    }
    // 능력치 창 토글 입력 핸들러 추가
    void OnToggleAbilityWindow(InputValue value)
    {
        if (value.isPressed)
        {
            OnToggleAbilityWindowEvent?.Invoke();
        }
    }

    void OnToggleShadow(InputValue value)
    {
        if (value.isPressed)
        {
            OnToggleShadowEvent?.Invoke();
        }
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            OnAttackEvent?.Invoke();
        }
    }

    void OnChangeShearsForm(InputValue value)
    {
        if (value.isPressed)
        {
            OnChangeFormEvent?.Invoke(WeaponType.Shears);
        }
    }

    void OnChangeTwinSwordForm(InputValue value)
    {
        
        if (value.isPressed)
        {
            OnChangeFormEvent?.Invoke(WeaponType.TwinSword);
        }
    }

    void OnChangeGreatSwordForm(InputValue value)
    {
        
        if (value.isPressed)
        {
            OnChangeFormEvent?.Invoke(WeaponType.GreatSword);
        }
    }
    
    void OnInventory(InputValue value)
    {
        if (value.isPressed)
        {
            OnInventoryEvent?.Invoke();
        }
    }
    
    // 사망 시 호출되어 플레이어 입력 비활성화
    private void DisableInput()
    {
        // Input System 비활성화
        if (inputSystem != null)
        {
            inputSystem.actions.Disable(); 
        }
        
        // 스크립트 비활성화
        enabled = false; 
        
        // 이동 벡터 초기화
        moveVector = Vector2.zero; 
        
        Debug.Log("PlayerInput Disabled.");
    }
}
