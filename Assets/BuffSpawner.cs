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

    private List<CharacterBuffs.BuffTypes> _listOfBuffs = new List<CharacterBuffs.BuffTypes>();
    private List<float> _chancesOfBuffs = new List<float>();

    private CharacterCombat _characterCombat;

    [System.Serializable]
    public class BuffChances
    {
        public CharacterBuffs.BuffTypes _buffTypes;
        public float _buffChances;
        public GameObject _buffPrefab;
    }

    private void Start()
    {
        _characterCombat = FindObjectOfType<CharacterCombat>();
        SpawnBuff(this.transform);
    }

    public  void SpawnBuff(Transform position)
    {
        ResetValues();
        bool _foundSpawn = false;
        switch (_characterCombat.PCurrentWeaponType)
        {
            case CharacterCombat.WeaponTypes.Sword:
                for(int i = 0; i < _swordBuffs.Count; i++)
                {
                    _listOfBuffs.Add(_swordBuffs[i]._buffTypes);
                    _chancesOfBuffs.Add(_swordBuffs[i]._buffChances * (_chancesForMainWeap / 100));
                }

                for (int i = 0; i < _generalBuffs.Count; i++)
                {
                    _listOfBuffs.Add(_generalBuffs[i]._buffTypes);
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
                            _foundSpawn = true;
                            Debug.Log(_listOfBuffs[j]);
                            Debug.Log(j);
                            Debug.Log(_randomNum);
                            j = 0;
                            break;
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
