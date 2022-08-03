using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateBase : StateMachineBehaviour
{
    private CharacterCombat _characterCombat;
    public CharacterCombat GetCharacterCombat(Animator animator)
    {
        if(_characterCombat == null)
        {
            _characterCombat = animator.GetComponentInParent<CharacterCombat>();
        }

        return _characterCombat;
    }
}
