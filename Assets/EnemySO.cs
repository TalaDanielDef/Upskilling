using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]

public class EnemySO : ScriptableObject
{
    public enum EnemyType { Melee, Range }
    public EnemyType _enemyType;
    public int _playerAggroRange;
    public int _maxHP;
    public float _timeBtwnAttacks;
    public float _attackRange;
    public GameObject _attackPrefab;
    public float _stoppingRange;
}
