using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _currentHp;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _player;
    [SerializeField] private int _playerAggroRange;
    [SerializeField] private EnemyState _currentState;
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private float _timeBtwnAttacks;
    public enum EnemyState { Roaming, Attack, Idle}
    public enum EnemyType { Melee, Range}
    private void Start()
    {
        _currentHp = _maxHp;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        //_navMeshAgent.SetDestination(_player.transform.position);
    }

    public void ReduceHP(int hpReduce)
    {
        _currentHp -= hpReduce;
        _hpBar.value = (float)_currentHp / (float)_maxHp;
    }

    public void Update()
    {
        float _distanceToPlayer = Vector3.Distance(this.transform.position, _player.transform.position);
        Debug.Log(_distanceToPlayer);
        if(_playerAggroRange > _distanceToPlayer && _currentState != EnemyState.Attack)
        {
            _currentState = EnemyState.Attack;
        }

        switch (_currentState)
        {
            case EnemyState.Attack:
                _navMeshAgent.SetDestination(_player.transform.position);
                break;
            case EnemyState.Roaming:
                break;
            case EnemyState.Idle:
                break;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, _playerAggroRange);
    }
}
