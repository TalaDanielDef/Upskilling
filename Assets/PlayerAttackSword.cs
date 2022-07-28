using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSword : CharacterStateBase
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("BackToMove", true);
        GetCharacterCombat(animator)._animationFlag = true;
        GetCharacterCombat(animator)._isAttacking = true;
        GetCharacterCombat(animator)._animationTrigger = false;
        GetCharacterCombat(animator).RegisterWeaponDamage();
        GetCharacterCombat(animator).StartDashFromWeapon();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GetCharacterCombat(animator)._isAttacking = true;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GetCharacterCombat(animator)._isAttacking = false;
        //GetCharacterMovement(animator)._animationFlag = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
