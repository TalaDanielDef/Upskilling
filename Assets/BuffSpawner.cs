using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [Header("Sword Buffs")]
    [SerializeField] private List<BuffChances> _swordBuffs;

    [Header("Bow Buffs")]
    [SerializeField] private List<BuffChances> _bowBuffs;

    [Header("Funnel Buffs")]
    [SerializeField] private List<BuffChances> _funnelBuffs;

    [Header("General Buffs")]
    [SerializeField] private List<BuffChances> _generalBuffs;

    [SerializeField] private float _chancesForMainWeap, _chancesForGeneral;

    private List<BuffChances> _listOfBuffs = new List<BuffChances>();
    private List<float> _chancesOfBuffs = new List<float>();

    private CharacterCombat _characterCombat;

    [SerializeField] private List<GameObject> _spawnedObjects;

    [System.Serializable]
    public class BuffChances
    {
        public CharacterBuffs.BuffTypes _buffTypes;
        public float _buffChances;
        public GameObject _buffPrefab;
        public int _buffLimit;
    }

    private void Start()
    {
        _characterCombat = FindObjectOfType<CharacterCombat>();
        //SpawnBuff(this.transform);
    }

    public void CheckSpawnObjects()
    {
        for(int i = 0; i < _spawnedObjects.Count; i++)
        {
            if(_spawnedObjects[i] == null)
            {
                _spawnedObjects.Remove(_spawnedObjects[i]);
            }
        }
    }
    public  void SpawnBuff(Transform position)
    {
        ResetValues();
        CheckSpawnObjects();
        bool _foundSpawn = false;
        switch (_characterCombat.PCurrentWeaponType)
        {
            case CharacterCombat.WeaponTypes.Sword:
                for(int i = 0; i < _swordBuffs.Count; i++)
                {
                    _listOfBuffs.Add(_swordBuffs[i]);
                    _chancesOfBuffs.Add(_swordBuffs[i]._buffChances * (_chancesForMainWeap / 100));
                }

                for (int i = 0; i < _generalBuffs.Count; i++)
                {
                    _listOfBuffs.Add(_generalBuffs[i]);
                    _chancesOfBuffs.Add(_generalBuffs[i]._buffChances * (_chancesForGeneral / 100));
                }

                while (_foundSpawn == false)
                {

                    float _randomNum = 0;
                    _randomNum = Random.Range(0, 101);
                    //int _randomNum = UnityEngine.Random.Range(1, 100);
                    for (int j = 0; j < _listOfBuffs.Count; j++)
                    {
                        float _sumOfChances = 0;
                        for (int k = j; k >= 0; k--)
                        {
                            _sumOfChances += _chancesOfBuffs[k];
                        }
                        if ((_randomNum <= _sumOfChances) && (_randomNum >= (_sumOfChances - _chancesOfBuffs[j])))
                        {
                            int _numberOfBuffs = 0;
                            for(int i = 0; i < CharacterBuffs.PInstance.PCurrentBuffs.Count; i++)
                            {
                                if(_listOfBuffs[j]._buffTypes == CharacterBuffs.PInstance.PCurrentBuffs[i]._buffName)
                                {
                                    _numberOfBuffs = CharacterBuffs.PInstance.PCurrentBuffs[i]._buffCount;
                                }
                            }

                            for (int i = 0; i < _spawnedObjects.Count; i++)
                            {
                                if(_spawnedObjects[i] != null)
                                {
                                    if (_spawnedObjects[i].GetComponent<BuffItemScript>().PPlayerBuff == _listOfBuffs[j]._buffTypes)
                                    {
                                        _numberOfBuffs++;
                                    }
                                }
                            }
                            if(_listOfBuffs[j]._buffLimit > _numberOfBuffs)
                            {
                                _foundSpawn = true;
                                _spawnedObjects.Add(Instantiate(_listOfBuffs[j]._buffPrefab, this.transform, false));
                                j = 0;
                                break;
                            }

                        }
                    }
                }

                break;
        }

    }

    public void ResetValues()
    {
        _listOfBuffs.Clear();
        _chancesOfBuffs.Clear();
    }




}
