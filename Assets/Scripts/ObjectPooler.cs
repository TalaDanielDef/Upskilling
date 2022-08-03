using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler _instance;
    [System.Serializable]
    public class Pool
    {
        public string _tag;
        public GameObject _prefab;
        public int _size;
    }


    public List<Pool> _pools;
    public Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool _pool in _pools)
        {
            Queue<GameObject> _objectPool = new Queue<GameObject>();
            
            for (int i = 0; i < _pool._size; i++)
            {
                GameObject _object = Instantiate(_pool._prefab);
                _object.SetActive(false);
                _objectPool.Enqueue(_object);
            }

            _poolDictionary.Add(_pool._tag, _objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        if(!_poolDictionary.ContainsKey(tag))
        {
            return null;
        }
        GameObject _objectToSpawn = _poolDictionary[tag].Dequeue();
        _objectToSpawn.SetActive(true);
        _objectToSpawn.transform.position = position;
        _objectToSpawn.transform.rotation = rotation;

        IPooledObject _pooledObject = _objectToSpawn.GetComponent<IPooledObject>();
        if(_pooledObject != null)
        {
            _pooledObject.OnObjectSpawn();
        }
        _poolDictionary[tag].Enqueue(_objectToSpawn);
        return _objectToSpawn;
    }

}
