using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// 플레이어의 이동, 점프, 대시 입력을 감지하고 상태를 관리하는 클래스  
/// Input System actions를 통해 입력 처리
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private Vector2 moveVector;
    
    // 플레이어 입력 이벤트 선언 (점프/대시)
    public event Action OnJumpEvent;
    public event Action OnDashEvent; 

    #region Player Input Properties
    public Vector2 MoveVector => moveVector;
    #endregion

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
}
