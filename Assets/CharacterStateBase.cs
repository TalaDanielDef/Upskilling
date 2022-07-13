using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateBase : StateMachineBehaviour
{
    private CharacterMovement _characterMovement;
    public CharacterMovement GetCharacterMovement(Animator animator)
    {
        if(_characterMovement == null)
        {
            _characterMovement = animator.GetComponentInParent<CharacterMovement>();
        }

        return _characterMovement;
    }
}
