using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack1 : CharacterStateBase
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("BackToMove", true);
        GetCharacterMovement(animator)._animationFlag = true;
        GetCharacterMovement(animator)._isAttacking = true;
        GetCharacterMovement(animator)._animationTrigger = false;
        Debug.Log("Animation 1");
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GetCharacterMovement(animator)._isAttacking = true;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GetCharacterMovement(animator)._isAttacking = false;
        //GetCharacterMovement(animator)._animationFlag = false;

        Debug.Log("Animation 2");
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