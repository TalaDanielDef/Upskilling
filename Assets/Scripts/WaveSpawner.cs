using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    private static WaveSpawner _instance;

    [SerializeField] private int _waveCount = 0;
    [SerializeField] private int _insideWaveCount = 0;
    [SerializeField] private List<WaveInfo> _waveInfo;
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
    [SerializeField] private GameObject _enemyBasePrefab;
    [SerializeField] private GameObject _stageDimensions;
    [SerializeField] private Button _waveResetButton;
    [SerializeField] private TextMeshProUGUI _waveCountText;
    private int _enemyCount;
    private bool _spawningInsideWaves = false;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        StartWaves();
        _waveResetButton.interactable = false;
        _waveCountText.text = "1";
    }

    public void StartWaves()
    {
        StartCoroutine(StartSpawningWave());
    }

    public void UpdateText()
    {
        _waveCountText.text = (_waveCount + 1).ToString();
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
            UpdateText();
            yield return new WaitForSeconds(_waveInfo[_waveCount].PTimerForNextWave);
            StartCoroutine(StartSpawningInsideWave());
            yield return new WaitUntil(() => _spawningInsideWaves == false);
            _waveCount++;
            Debug.Log("Testt" + _waveCount + ":" + _waveInfo.Count);
            if (_waveCount + 1 > _waveInfo.Count)
            {
                _waveResetButton.interactable = true;
                //ResetWave();
            }
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
                if(_enemies.Count != 0)
                {
                    _enemies.Clear();
                    CharacterCombat.PInstance.PEnemiesInRange.Clear();
                }

                for(int i = 0; i < _waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO.Count; i++)
                {
                    GameObject _enemy = Instantiate(_enemyBasePrefab, this.transform.position, Quaternion.identity);
                    _enemies.Add(_enemy);
                    _enemy.GetComponent<EnemyScript>().PEnemySO = _waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO[i];
                    _enemy.transform.position = GetRandomPosition();

                }
                CharacterCombat.PInstance.PEnemies = _enemies;
                SetEnemyCount(_waveInfo[_waveCount].PListInsideWave[_insideWaveCount].PEnemySO.Count);
                _insideWaveCount++;
            }
        }
        _insideWaveCount = 0;
        yield return new WaitUntil(() => !HaveEnemies());
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

    public void ResetWave()
    {
        _waveCount = 0;
        Start();
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
    #endregion

    public int PEnemyCount { get { return _enemyCount; } set { _enemyCount = value; } }
    public static WaveSpawner PInstance { get { return _instance; } }
    public List<GameObject> PEnemies { get { return _enemies; } }

}
