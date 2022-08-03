using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string _weaponName;

    public CharacterCombat.WeaponTypes _weaponType;

    [Header("Sword")]
    public float[] _weaponDashPerAttack;
    public int[] _weaponDamagePerAttack;
    public int[] _weaponKnockbackPerAttack;

    [Header("Bow")]
    public float _bowRange;
    public GameObject _arrowPrefab;

    [Header("Fin Funnels")]
    public float _enemyDetectionRange;
    public int _damagePerHitFunnel;
    public int _funnelCount;
    public GameObject _funnelPrefab;


}
