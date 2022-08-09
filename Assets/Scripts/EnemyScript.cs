using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private int _currentHp;
    [SerializeField] private bool _isKnockback;
    [SerializeField] private bool _isShooting = false;
    [SerializeField] private float _timerToShoot;
    [SerializeField] private bool _isDoneShooting = false;
    [SerializeField] private float _roamTimer;
    [SerializeField] private float _roamRadius;
    [SerializeField] private GameObject _bulletPlace;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject[] _bodyTypes;
    [SerializeField] private EnemyState _currentState;
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private EnemyBodyType _currentEnemyBodyType;
    [SerializeField] private BoxCollider _collider;
    public float turnSpeed = .01f;
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rb;
    private float _attackTimer;
    private float _multiplier = 1;
    private float _currentRoamTimer = 0;
    private bool _isHit = false;
    private bool _isDestroy = false;
    private bool _isSlam = false;
    private bool _isDoneSlam = false;
    private bool _isDashing = false;
    private bool _dashDamage = false;
    private Quaternion rotGoal;
    private Vector3 direction;
    private Vector3 _knockbackPos;
    public enum EnemyState { Roaming, Attack, Idle}
    public enum EnemyBodyType { Humanoid, Ball}

    private void Start()
    {
        _currentHp = _enemySO._maxHP;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _navMeshAgent.stoppingDistance = _enemySO._stoppingRange;
        _player = GameObject.FindGameObjectWithTag("Player");
        _currentEnemyBodyType = _enemySO._enemyBodyType;
        switch (_currentEnemyBodyType)
        {
            case EnemyBodyType.Humanoid:
                _bodyTypes[0].SetActive(true);
                break;
            case EnemyBodyType.Ball:
                _bodyTypes[1].SetActive(true);
                break;

            default:
                break;
        }
    }

    public void ReduceHP(int hpReduce)
    {
        _isHit = true;
        _currentHp -= hpReduce;
        _hpBar.value = (float)_currentHp / (float)_enemySO._maxHP;

        if(_currentHp <= 0)
        {
            if(!_isDestroy)
            {
                WaveSpawner.PInstance.PEnemyCount--;
                WaveSpawner.PInstance.HaveEnemies();
                _isDestroy = true;
            }

            Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        float _distanceToPlayer = Vector3.Distance(this.transform.position, _player.transform.position);
        //Debug.Log(_distanceToPlayer);
        if((_enemySO._playerAggroRange > _distanceToPlayer && _currentState != EnemyState.Attack) || _isHit)
        {
            _currentState = EnemyState.Attack;
        }

        if(_enemySO._playerAggroRange < _distanceToPlayer && _currentState != EnemyState.Roaming && !_isHit)
        {
            _currentState = EnemyState.Roaming;
        }
        switch (_currentState)
        {
            case EnemyState.Attack:
                //_navMeshAgent.SetDestination(_player.transform.position);
                if(_navMeshAgent.stoppingDistance == 0)
                {
                    _navMeshAgent.stoppingDistance = _enemySO._stoppingRange;
                }
                switch (_enemySO._enemyType)
                {
                    case EnemySO.EnemyType.Melee:
                        _attackTimer += Time.deltaTime;
                        if (_enemySO._attackRange > _distanceToPlayer)
                        {
                            if (_attackTimer >= _enemySO._timeBtwnAttacks && !_isKnockback)
                            {
                                direction = (_player.transform.position - transform.position).normalized;
                                rotGoal = Quaternion.LookRotation(direction);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);
                                _isSlam = true;
                                if (!_isDoneSlam)
                                {
                                    StartCoroutine(Slam());
                                }
                            }
                        }
                        break;
                    case EnemySO.EnemyType.Range:
                        _attackTimer += Time.deltaTime;
                        if (_enemySO._attackRange > _distanceToPlayer)
                        {
                            if (_attackTimer >= _enemySO._timeBtwnAttacks && !_isKnockback)
                            {
                                direction = (_player.transform.position - transform.position).normalized;
                                rotGoal = Quaternion.LookRotation(direction);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);
                                _isShooting = true;
                                if (!_isDoneShooting)
                                {
                                    StartCoroutine(Shoot());
                                }
                            }
                        }
                        break;
                    case EnemySO.EnemyType.Charging:
                        _attackTimer += Time.deltaTime;
                        if (_enemySO._attackRange > _distanceToPlayer)
                        {
                            if (_attackTimer >= _enemySO._timeBtwnAttacks)
                            {
                                _collider.enabled = false;
                                _isDashing = true;
                                this.transform.LookAt(_player.transform);
                                if(_isDashing)
                                {
                                    _dashDamage = true;
                                    StartCoroutine(Dash());
                                    _isDashing = false;
                                }
                                _attackTimer = 0;
                            }
                        }
                        break;
                }
                break;
            case EnemyState.Roaming:
                _navMeshAgent.stoppingDistance = 0;
                _currentRoamTimer += Time.deltaTime;
                if(_currentRoamTimer >= _roamTimer)
                {
                    _navMeshAgent.SetDestination(FindRoamDestination(_roamRadius));
                    _currentRoamTimer = 0;
                }
                break;
            case EnemyState.Idle:
                break;
        }
        if(!_isKnockback)
        {
            _navMeshAgent.angularSpeed = 200;
        }

        if(_enemySO._backingRange > _distanceToPlayer && !_isShooting && !_isDashing)
        {
            Vector3 _dirToPlayer = transform.position - _player.transform.position;
            Vector3 _minimizedDirection = new Vector3(0f, 0f, 0f);

            if (_dirToPlayer.x > 0)
                _minimizedDirection.x = 1;
            if (_dirToPlayer.x < 0)
                _minimizedDirection.x = -1;
            if (_dirToPlayer.z > 0)
                _minimizedDirection.z = 1;
            if (_dirToPlayer.z < 0)
                _minimizedDirection.z = -1;


            Vector3 _newPos = transform.position + _minimizedDirection * _multiplier;
            _navMeshAgent.stoppingDistance = 0;
            _navMeshAgent.SetDestination(_newPos);
        }
        else if((_enemySO._playerAggroRange > _distanceToPlayer) || _isHit && !_isDashing)
        {
            _navMeshAgent.stoppingDistance = _enemySO._stoppingRange;
            _navMeshAgent.SetDestination(_player.transform.position);   
        }
    }

    IEnumerator Shoot()
    {
        _isDoneShooting = true;
        yield return new WaitForSeconds(_timerToShoot);
        ObjectPooler._instance.SpawnFromPool("Bullet", _bulletPlace.transform.position, Quaternion.identity);
        _isDoneShooting = false;
        _attackTimer = 0;
        _isShooting = false;
    }

    IEnumerator Slam()
    {
        _isDoneSlam = true;
        yield return new WaitForSeconds(_timerToShoot);
        Instantiate(_enemySO._attackPrefab, this.transform, false);
        _isDoneSlam = false;
        _attackTimer = 0;
        _isSlam = false;

    }

    IEnumerator Dash()
    {
        float _startTime = Time.time;
        while (Time.time < _startTime + _enemySO._dashTimer)
        {
            _navMeshAgent.Move(transform.forward * _enemySO._dashDistance * Time.deltaTime);
            yield return null;
        }
        _dashDamage = false;
        _collider.enabled = true;
    }

    public Vector3 FindRoamDestination(float radius)
    {
        Vector3 _finalPosition = Vector3.zero;
        while(_finalPosition == Vector3.zero)
        {
            Vector3 _randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            _randomDirection += transform.position;
            NavMeshHit _hit;

            if(NavMesh.SamplePosition(_randomDirection, out _hit, radius, 1))
            {
                _finalPosition = _hit.position;
            }
        }
        return _finalPosition;
    }
    public void KnockBack(int knockbackPower)
    {
        _knockbackPos = new Vector3(0f, 0f, 0f);
        _knockbackPos = _player.transform.forward;
        _navMeshAgent.angularSpeed = 0;
        _navMeshAgent.velocity = _knockbackPos * knockbackPower;
        
        _isKnockback = true;
        StopCoroutine(StartKnockBack());
        StartCoroutine(StartKnockBack());
    }

    IEnumerator StartKnockBack()
    {
        yield return new WaitForSeconds(1f);
        if(_isKnockback)
        {
            _isKnockback = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(this.transform.position, _enemySO._playerAggroRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_enemySO._enemyType == EnemySO.EnemyType.Charging)
        {
            if(other.tag.Equals("Player"))
            {
                if(_dashDamage)
                other.GetComponent<CharacterHealth>().ReduceHP(_enemySO._dashDamage);
            }
        }
    }

    public EnemySO PEnemySO { set { _enemySO = value; } }
}
