using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinFunnels : MonoBehaviour
{
    [SerializeField] private float _timeBtwnShots;
    [SerializeField] private float _laserDuration;
    [SerializeField] private float _enemyRadius;
    [SerializeField] private float _delayMovement;
    [SerializeField] private float _funnelSpeed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private GameObject _laserPosition;
    [SerializeField] private LineRenderer _lineRend;
    [SerializeField] private List<GameObject> _enemies;
    private float _timer;
    private bool _generateRandom = true;
    private bool _startFindingEnemies = false;
    private int _randomEnemy = 0;
    private bool _firstSpawn = true;
    private bool _goingBack = false;
    private int _damageToEnemy;
    private int _previousTarget;
    private GameObject _initialPosition;
    private GameObject _outPosition;
    private Rigidbody _rb;
    private Vector3 _randomPosition;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_firstSpawn)
        {
            transform.position = Vector3.MoveTowards(transform.position, _initialPosition.transform.position, Time.deltaTime * _funnelSpeed);
            transform.LookAt(_initialPosition.transform);
            if (transform.position == _initialPosition.transform.position)
            {
                _startFindingEnemies = true;
                _firstSpawn = false;
            }
        }
        else if(_enemies.Count != 0)
        {
            if (_startFindingEnemies)
            {
                if (_enemies.Count > 0)
                {
                    _timer += Time.deltaTime;

                    if(_enemies[_previousTarget] == null)
                    {
                        _generateRandom = true;
                        _randomPosition = Vector3.zero;
                    }
                    if (_generateRandom)
                    {
                        _randomEnemy = (int)Random.Range(0, _enemies.Count);
                        if (_enemies[_randomEnemy] == null)
                        {
                            //_enemies.RemoveAt(_randomEnemy);
                            return;
                        }
                        Debug.Log(_enemies.Count);
                        Debug.Log(_randomEnemy);
                        _previousTarget = _randomEnemy;

                        while (_randomPosition.y < 1.4f)
                            _randomPosition = (Random.onUnitSphere * _enemyRadius) + _enemies[_randomEnemy].transform.position;

                        _generateRandom = false;
                    }


                    transform.position = Vector3.MoveTowards(transform.position, _randomPosition, Time.deltaTime * _funnelSpeed);
                    if(_enemies[_randomEnemy] != null)
                    {
                        var direction = (_enemies[_randomEnemy].transform.position - transform.position).normalized;
                        var rotGoal = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _turnSpeed);
                    }
                    else
                    {
                        return;
                    }

                    //transform.LookAt(_enemies[_randomEnemy].transform.GetChild(5).gameObject.transform.position);
                    if (_timeBtwnShots <= _timer)
                    {
                        if(_enemies[_randomEnemy] == null)
                        {
                            _randomPosition = Vector3.zero;
                            _generateRandom = true;
                            return;
                        }

                        if(_randomEnemy > _enemies.Count - 1)
                        {
                            _randomPosition = Vector3.zero;
                            _generateRandom = true;
                            return;
                        }
                        if (transform.position == _randomPosition)
                        {
                            _lineRend.SetPosition(0, _laserPosition.transform.position);
                            _lineRend.SetPosition(1, _enemies[_randomEnemy].transform.GetChild(5).gameObject.transform.position);
                            StartCoroutine(ShootLaser());
                            _enemies[_randomEnemy].GetComponent<EnemyScript>().ReduceHP(_damageToEnemy);
                            _timer = 0;
                        }

                    }
                }
            }
        }
        //interna 
        else
        {
            _outPosition = CharacterCombat.PInstance.POutPos;
            _initialPosition = CharacterCombat.PInstance.PInitialPos;
            _generateRandom = true;
            _randomPosition = Vector3.zero;
            if(!_goingBack)
            {
                transform.position = Vector3.MoveTowards(transform.position, _initialPosition.transform.position, Time.deltaTime * _funnelSpeed);
                transform.LookAt(_initialPosition.transform);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _outPosition.transform.position, Time.deltaTime * _funnelSpeed);
                transform.LookAt(_outPosition.transform);
                if(transform.position == _outPosition.transform.position)
                {
                    Destroy(this.gameObject);
                }
            }
            if (transform.position == _initialPosition.transform.position)
            {
                _goingBack = true;
            }
        }
    }

    IEnumerator ShootLaser()
    {
        _lineRend.enabled = true;
        yield return new WaitForSeconds(_laserDuration);
        _lineRend.enabled = false;
        _randomPosition = Vector3.zero;
        _generateRandom = true;
    }

    IEnumerator MoveFunnel()
    {
        if (transform.position != _randomPosition && _randomPosition != Vector3.zero)
        {
            yield return new WaitForSeconds(_delayMovement);
            transform.position = _randomPosition;
            _randomPosition = Vector3.zero;
        }
    }

    public List<GameObject> PEnemies { set { _enemies = value; } }
    public float PFunnelSpeed { get { return _funnelSpeed; } }
    public bool PFunnelStart { set { _startFindingEnemies = value; } }
    public GameObject PInitialPos { set { _initialPosition = value; } }
    public int PDamageToEnemy { set { _damageToEnemy = value; } }
}
