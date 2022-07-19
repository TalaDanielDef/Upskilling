using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string _weaponName;
    public float[] _weaponDashPerAttack;
    public int[] _weaponDamagePerAttack;
    public float[] _weaponKnockbackPerAttack;
    // float[] _weapon
    public CharacterMovement.WeaponTypes _weaponType;

}
