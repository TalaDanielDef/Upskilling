using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    private static WaveSpawner _instance;
    [SerializeField] private List<WaveInfo> _waveInfo;
    private GameObject[] _enemies;
    private float _timer;
    [SerializeField] private int _waveCount = 0;
    [SerializeField] private int _insideWaveCount = 0;
    [SerializeField] private GameObject _enemyBasePrefab;
    [SerializeField] private GameObject _stageDimensions;
    private bool _noEnemies;
    private int _enemyCount;
    private bool _spawningInsideWaves = false;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        StartWaves();
    }

    public void StartWaves()
    {
        StartCoroutine(StartSpawningWave());
    }

    public bool HaveEnemies()
    {
        if(_enemyCount <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetEnemyCount(int _currentEnemyCount)
    {
        _enemyCount = _currentEnemyCount;
    }

    IEnumerator StartSpawningWave()
    {
        yield return new WaitForSeconds(2f);
        while (_waveCount < _waveInfo.Count)
        {
            Debug.Log("3321");
            yield return new WaitForSeconds(_waveInfo[_waveCount].PTimerForNextWave);
            StartCoroutine(StartSpawningInsideWave());
            yield return new WaitUntil(() => _spawningInsideWaves == false);
            Debug.Log("3322");
            _waveCount++;
        }

    }

    IEnumerator StartSpawningInsideWave()
    {
        _spawningInsideWaves = true;
        while (_insideWaveCount < _waveInfo[_waveCount].PListInsideWave.Count)
        {
            yield return new WaitUntil(() => !HaveEnemies());
            yield return new WaitForSeconds(_waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PTimerForNextInsideWave);
            {
                for(int i = 0; i < _waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO.Count; i++)
                {
                    GameObject _enemy = Instantiate(_enemyBasePrefab, this.transform.position, Quaternion.identity);
                    _enemy.GetComponent<EnemyScript>().PEnemySO = _waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO[i];
                    _enemy.transform.position = GetRandomPosition();
                }

                SetEnemyCount(_waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO.Count);
                _insideWaveCount++;
            }
        }
        _insideWaveCount = 0;
        yield return new WaitUntil(() => !HaveEnemies());
        Debug.Log("3322");
        _spawningInsideWaves = false;
    }

    public Vector3 GetRandomPosition()
    {
        bool _foundRandomPosition = false;
        Vector2 _xValues = MinMaxOfXValue();
        Vector2 _zValues = MinMaxOfZValue();
        Vector3 _finalPosition = Vector3.zero;
        while(!_foundRandomPosition)
        {
            float _xRandom = Random.Range(_xValues.x, _xValues.y);
            float _zRandom = Random.Range(_zValues.x, _zValues.y);
            Vector3 _randomPosition = new Vector3(_xRandom, 0, _zRandom);

            NavMeshHit _hit;
            if (NavMesh.SamplePosition(_randomPosition, out _hit, 10000, 1))
            {
                _finalPosition = _randomPosition;
                _foundRandomPosition = true;
            }
        }

        return _finalPosition;
    }

    public Vector2 MinMaxOfXValue()
    {
        Vector2 _minMaxOfX = new Vector2(0f, 0f);
        float _highX = 0, _lowX = 0;
        foreach(Transform _child in _stageDimensions.transform)
        {
            if(_highX < _child.transform.position.x)
            {
                _highX = _child.transform.position.x;
            }

            if(_lowX > _child.transform.position.x)
            {
                _lowX = _child.transform.position.x;
            }
        }

        _minMaxOfX = new Vector2(_highX, _lowX);

        return _minMaxOfX;
    }

    public Vector2 MinMaxOfZValue()
    {
        Vector2 _minMaxOfZ = new Vector2(0f, 0f);
        float _highZ = 0, _lowZ = 0;
        foreach (Transform _child in _stageDimensions.transform)
        {
            if (_highZ < _child.transform.position.z)
            {
                _highZ = _child.transform.position.z;
            }

            if (_lowZ > _child.transform.position.z)
            {
                _lowZ = _child.transform.position.z;
            }
        }

        _minMaxOfZ = new Vector2(_highZ, _lowZ);

        return _minMaxOfZ;
    }


    #region Classes
    [System.Serializable]
    public class WaveInfo
    {
        [SerializeField] private float _timerForNextWave;
        [SerializeField] private List<InsideWaveInfo> _insideWaveInfos;

        public float PTimerForNextWave { get { return _timerForNextWave; } }
        public List<InsideWaveInfo> PListInsideWave { get { return _insideWaveInfos; } }
    }

    [System.Serializable]
    public class InsideWaveInfo
    {
       [SerializeField] private float _timerForNextInsideWave;
       [SerializeField] private List<EnemySO> _enemySO;

        public float PTimerForNextInsideWave { get { return _timerForNextInsideWave; } }
        public List<EnemySO> PEnemySO { get { return _enemySO; } }
    }

    public int PEnemyCount { get { return _enemyCount; } set { _enemyCount = value; } }
    public static WaveSpawner PInstance { get { return _instance; } }
    #endregion
}
