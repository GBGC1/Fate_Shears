using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHandler : MonoBehaviour
{
    private InputAttackKey attackKey;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        attackKey = new InputAttackKey();
        attackKey.Attack.Enable();
        attackKey.Attack.BasicAttack.performed += BasicAttack;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void BasicAttack(InputAction.CallbackContext ctx)
    {
        StartBasicAttackAnimation();
        Debug.Log("InputBasicAttack");
    }

    private void StartBasicAttackAnimation()
    {
        if (!animator.GetBool("Atk"))
        {
            animator.SetBool("Atk", true);
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