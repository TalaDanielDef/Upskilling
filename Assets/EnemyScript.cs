using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int _currentHp;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _player;
    [SerializeField] private EnemyState _currentState;
    //[SerializeField] private EnemyType _enemyType;
    //[SerializeField] private float _timeBtwnAttacks;
    [SerializeField] private EnemySO _enemySO;
    private Rigidbody _rb;
    private Vector3 _knockbackPos;
    public enum EnemyState { Roaming, Attack, Idle}
    [SerializeField] private bool _isKnockback;
    private float _attackTimer;
    [SerializeField] private GameObject _bulletPlace;
    private float _multiplier = 1;
    private void Start()
    {
        _currentHp = _enemySO._maxHP;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _navMeshAgent.stoppingDistance = _enemySO._stoppingRange;
    }

    public void ReduceHP(int hpReduce)
    {
        _currentHp -= hpReduce;
        _hpBar.value = (float)_currentHp / (float)_enemySO._maxHP;
    }

    public void Update()
    {
        float _distanceToPlayer = Vector3.Distance(this.transform.position, _player.transform.position);
        //Debug.Log(_distanceToPlayer);
        if(_enemySO._playerAggroRange > _distanceToPlayer && _currentState != EnemyState.Attack)
        {
            _currentState = EnemyState.Attack;
        }
        switch (_currentState)
        {
            case EnemyState.Attack:
                //_navMeshAgent.SetDestination(_player.transform.position);
                break;
            case EnemyState.Roaming:
                break;
            case EnemyState.Idle:
                break;
        }
        if(!_isKnockback)
        {
            _navMeshAgent.angularSpeed = 120;
        }

        switch (_enemySO._enemyType)
        {
            case EnemySO.EnemyType.Melee:
                _attackTimer += Time.deltaTime;
                if (_enemySO._attackRange > _distanceToPlayer)
                {
                    if (_attackTimer >= _enemySO._timeBtwnAttacks && !_isKnockback)
                    {
                        Instantiate(_enemySO._attackPrefab, this.transform, false);
                        _attackTimer = 0;
                    }
                }
                break;
            case EnemySO.EnemyType.Range:
                _attackTimer += Time.deltaTime;
                //Debug.Log(_enemySO._attackRange);
                //Debug.Log(_distanceToPlayer);
                if (_enemySO._attackRange > _distanceToPlayer)
                {
                    if (_attackTimer >= _enemySO._timeBtwnAttacks && !_isKnockback)
                    {
                        //Instantiate(_enemySO._attackPrefab, _bulletPlace.transform.position, Quaternion.identity);
                        this.transform.LookAt(_player.transform);
                        ObjectPooler._instance.SpawnFromPool("Bullet", _bulletPlace.transform.position, Quaternion.identity);
                        _attackTimer = 0;
                    }
                }
                break;
        }

        if(_enemySO._backingRange > _distanceToPlayer)
        {
            Vector3 _dirToPlayer = transform.position - _player.transform.position;
            Vector3 _minimizedDirection = new Vector3(0f, 0f, 0f);

            Debug.Log(_dirToPlayer.x);
            Debug.Log(_dirToPlayer.z);
            if (_dirToPlayer.x > 0)
                _minimizedDirection.x = 1;
            if (_dirToPlayer.x < 0)
                _minimizedDirection.x = -1;
            if (_dirToPlayer.z > 0)
                _minimizedDirection.z = 1;
            if (_dirToPlayer.z < 0)
                _minimizedDirection.z = -1;

            Debug.Log(_minimizedDirection);
            //if (_dirToPlayer.magnitude > 0.5)
            //{
            //    _multiplier = 50;
            //}
            //else if (_dirToPlayer.magnitude > 1)
            //{
            //    _multiplier = 10;
            //}
            //else if(_dirToPlayer.magnitude > 1.5)
            //{
            //    _multiplier = 7;
            //}
            //else if(_dirToPlayer.magnitude > 2.5)
            //{
            //    _multiplier = 5;
            //}
            //else
            //{
            //    _multiplier = 1;
            //}

            Vector3 _newPos = transform.position + _minimizedDirection * _multiplier;

            //Debug.Log(_newPos);
            _navMeshAgent.stoppingDistance = 0;
            _navMeshAgent.SetDestination(_newPos);
        }
        else if(_enemySO._playerAggroRange > _distanceToPlayer )
        {
            _navMeshAgent.stoppingDistance = _enemySO._stoppingRange;
            _navMeshAgent.SetDestination(_player.transform.position);   
        }
        //if(_enemySO._enemyType == EnemySO.EnemyType.Melee)
        //{
        //    _attackTimer += Time.deltaTime;
        //    if(_enemySO._attackRange > _distanceToPlayer)
        //    {
        //        if(_attackTimer >= _enemySO._timeBtwnAttacks)
        //        {
        //            Instantiate(_enemySO._attackPrefab, this.transform, false);
        //            _attackTimer = 0;
        //        }
        //    }
        //}
    }

    public void KnockBack(int knockbackPower)
    {
        _knockbackPos = new Vector3(0f, 0f, 0f);
        //Vector3 _playerDifferenceToEnemy = this.transform.position - _player.transform.position;
        //if (_playerDifferenceToEnemy.x > 0)
        //    _knockbackPos.x = 1;
        //if (_playerDifferenceToEnemy.x < 0)
        //    _knockbackPos.x = -1;
        //if (_playerDifferenceToEnemy.z > 0)
        //    _knockbackPos.z = 1;
        //if (_playerDifferenceToEnemy.z < 0)
        //    _knockbackPos.z = -1;
        _knockbackPos = _player.transform.forward;
        //Debug.Log(this.transform.position - _player.transform.position);
        //Debug.Log(_knockbackPos);
        _navMeshAgent.angularSpeed = 0;
        _navMeshAgent.velocity = _knockbackPos * knockbackPower;
        
        _isKnockback = true;
        StopAllCoroutines();
        StartCoroutine(StartKnockBack());
    }

    IEnumerator StartKnockBack()
    {
        yield return new WaitForSeconds(.75f);
        if(_isKnockback)
        {
            //_navMeshAgent.angularSpeed = 120;
            _isKnockback = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, _enemySO._playerAggroRange);
    }
}
