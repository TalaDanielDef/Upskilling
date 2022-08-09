using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private CharacterCombat _characterCombat;
    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _characterCombat = FindObjectOfType<CharacterCombat>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Enemy"))
        {
            other.GetComponent<EnemyScript>().ReduceHP(_characterCombat._currentWeaponDamage + (int)CharacterBuffs.PInstance.SwordDamageBuff());
            _characterCombat.gameObject.GetComponent<CharacterHealth>().AddHP(CharacterBuffs.PInstance.SwordLifeSteal());
            _characterCombat._currentWeaponDamage = 0;
            if(_characterCombat._isAttacking)
            other.GetComponent<EnemyScript>().KnockBack(_characterCombat._currentWeaponKnockback);
            _characterCombat._currentWeaponKnockback = 0;
        }
    }
}
