using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Enemy"))
        {
            //Debug.Log("Hit" + _characterMovement._currentWeaponDamage);
            other.GetComponent<EnemyScript>().ReduceHP(_characterMovement._currentWeaponDamage);
            _characterMovement._currentWeaponDamage = 0;
            if(_characterMovement._isAttacking)
            other.GetComponent<EnemyScript>().KnockBack(_characterMovement._currentWeaponKnockback);
            _characterMovement._currentWeaponKnockback = 0;
        }
    }
}
