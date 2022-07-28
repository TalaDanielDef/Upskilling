using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string _weaponName;
    public float[] _weaponDashPerAttack;
    public int[] _weaponDamagePerAttack;
    public int[] _weaponKnockbackPerAttack;
    public float _bowRange;
    public GameObject _arrowPrefab;
    // float[] _weapon
    public CharacterCombat.WeaponTypes _weaponType;

}
