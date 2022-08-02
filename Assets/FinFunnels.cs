using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinFunnels : MonoBehaviour
{
    [SerializeField] private float _finFunnelRange;
    [SerializeField] private int _damage;
    [SerializeField] private LineRenderer _lineRend;
    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private float _timeBtwnShots;
    [SerializeField] private GameObject _laserPosition;
    [SerializeField] private float _laserDuration;
    private float _timer;
    private bool _startLaserTimer;
    private Vector3 _randomPosition;
    [SerializeField] private float _enemyRadius;
    [SerializeField] private float _delayMovement;
    private Rigidbody _rb;
    [SerializeField] private float _funnelSpeed;
    private bool _generateRandom = true;
    private bool _startFindingEnemies = false;
    private int _randomEnemy = 0;
    [SerializeField] private float _turnSpeed;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(_startFindingEnemies)
        {
            if(_enemies.Count > 0)
            {
                _timer += Time.deltaTime;
                if(_generateRandom)
                {
                    Debug.Log("In");
                    _randomEnemy = (int)Random.Range(0, _enemies.Count - 1);
                    Debug.Log(_randomEnemy);
                    if (_enemies[_randomEnemy] == null)
                        return;

                    while (_randomPosition.y < 1.4f)
                        _randomPosition = (Random.onUnitSphere * _enemyRadius) + _enemies[_randomEnemy].transform.position;
                    _generateRandom = false;
                }

                transform.position = Vector3.MoveTowards(transform.position, _randomPosition, Time.deltaTime * _funnelSpeed);
                var direction = (_enemies[_randomEnemy].transform.position - transform.position).normalized;
                var rotGoal = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _turnSpeed);
                //transform.LookAt(_enemies[_randomEnemy].transform.GetChild(5).gameObject.transform.position);
                if (_timeBtwnShots <= _timer)
                {
                    if(transform.position == _randomPosition)
                    {
                        _lineRend.SetPosition(0, _laserPosition.transform.position);
                        _lineRend.SetPosition(1, _enemies[_randomEnemy].transform.GetChild(5).gameObject.transform.position);
                        StartCoroutine(ShootLaser());
                        _timer = 0;
                    }

                }
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
}
