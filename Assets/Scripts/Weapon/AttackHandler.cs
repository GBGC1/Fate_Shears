using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.InputSystem;
using Script.Manager.Events;

namespace Weapon{
public class AttackHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    //private InputAttackKey attackKey;
    private Animator animator;
    
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.OnAttackEvent += BasicAttack;
        playerInput.OnChangeFormEvent += ChangeForm;
        animator = GetComponent<Animator>();
        /*
        attackKey = new InputAttackKey();
        attackKey.Attack.Enable();
        attackKey.Attack.BasicAttack.performed += BasicAttack;*/
    }

    private void ChangeForm(WeaponType type)
    {
        WeaponBody.ChangeForm(type);
        EventBus.Instance().Publish(new ChangeWeaponEventData(type));
    }

    private void BasicAttack()
    {
        StartBasicAttackAnimation();
        Debug.Log("InputBasicAttack");
    }

    private void StartBasicAttackAnimation()
    {
        if (!animator.GetBool("Atk"))
        {
            animator.SetBool("Atk", true);
            animator.SetInteger("WeaponForm", (int)WeaponBody.CurrentForm);
        }
        else
        {
            animator.SetBool("NextAtk", true);
        }
    }

    private void ResetNextAtk()
    {
        animator.SetBool("NextAtk", false);
    }
    
    private void EndMotion()
    {
        animator.SetBool("Atk", false);
    }
}

}