using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]

public class EnemySO : ScriptableObject
{
    public enum EnemyType { Melee, Range, Charging }
    public EnemyType _enemyType;
    public EnemyScript.EnemyBodyType _enemyBodyType;
    public int _playerAggroRange;
    public int _maxHP;
    public float _timeBtwnAttacks;
    public float _attackRange;
    public GameObject _attackPrefab;
    public float _stoppingRange;
    public float _backingRange;
    public float _dashTimer;
    public float _dashDistance;
}
