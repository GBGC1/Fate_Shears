using System;
using Script.Manager.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Weapon;

public class AttackHandler : MonoBehaviour
{
    private InputAttackKey attackKey;
    private Animator animator;

    public static WeaponType CurrentForm { get; private set; } = WeaponType.Shears;

    void Start()
    {
        animator = GetComponent<Animator>();
        attackKey = new InputAttackKey();
        attackKey.Attack.Enable();
        attackKey.Attack.BasicAttack.performed += BasicAttack;
        attackKey.Attack.ChangeForm.performed += ChangeForm;
    }

    private void BasicAttack(InputAction.CallbackContext ctx)
    {
        StartBasicAttackAnimation();
        Debug.Log("InputBasicAttack");
    }

    private void ChangeForm(InputAction.CallbackContext ctx)
    {
        int next = ((int)CurrentForm + 1) % Enum.GetValues(typeof(WeaponType)).Length;
        CurrentForm = (WeaponType)next;
        EventBus.Instance().Publish(new ChangeWeaponEventData(CurrentForm.ToString()));
        WeaponBody.ChangeStat(CurrentForm);
    }

    private void StartBasicAttackAnimation()
    {
        if (!animator.GetBool("Atk"))
        {
            animator.SetBool("Atk", true);
            animator.SetInteger("WeaponForm", (int)CurrentForm);
            attackKey.Attack.ChangeForm.Disable();
        }
        else
        {
            animator.SetBool("NextAtk", true);
        }
    }

    private void ResetNextAtk()
    {
        Debug.Log("ResetNextAtk");
        animator.SetBool("NextAtk", false);
    }

    private void EndMotion()
    {
        animator.SetBool("Atk", false);
        attackKey.Attack.ChangeForm.Enable();
    }
}