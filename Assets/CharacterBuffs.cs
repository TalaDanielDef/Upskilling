using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuffs : MonoBehaviour
{
    private static CharacterBuffs _instance;

    [SerializeField] private List<Buffs> _currentBuffs;

    [SerializeField] private float _swordDamageMultiplier;
    [SerializeField] private float _swordLifeStealBase;
    [SerializeField] private float _bowRangeMultiplier;
    [SerializeField] private float _bowDamageMultiplier;
    [SerializeField] private float _funnelRangeMultiplier;
    [SerializeField] private int _funnelNumberBase;
    [SerializeField] private float _funnelDamageMultiplier;

    private void Awake()
    {
        _instance = this;
    }
    [System.Serializable]
    public class Buffs
    {
        public BuffTypes _buffName;
        public int _buffCount;
    }
    public enum BuffTypes
    {
        SwordUpDamage,
        SwordLifeSteal,
        BowUpRange,
        BowIncreaseArrow,
        BowIncreaseDamage,
        FunnelIncreaseRange,
        FunnelIncreaseNumber,
        FunnelIncreaseDamage,
        HealthIncrease,
        SpeedIncrease
    }

    public float SwordDamageBuff()
    {
        for(int i = 0; i < _currentBuffs.Count; i++)
        {
            if(_currentBuffs[i]._buffName == BuffTypes.SwordUpDamage)
            {
                return _currentBuffs[i]._buffCount * _swordDamageMultiplier;
            }
        }
        return 0;
    }

    public float SwordLifeSteal()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.SwordLifeSteal)
            {
                return _currentBuffs[i]._buffCount * _swordLifeStealBase;
            }
        }
        return 0;
    }
    
    public float BowUpRange()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.BowUpRange)
            {
                return _currentBuffs[i]._buffCount * _bowRangeMultiplier;
            }
        }
        return 0;
    }

    public float BowDamageBuff()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.BowIncreaseDamage)
            {
                return _currentBuffs[i]._buffCount * _bowDamageMultiplier;
            }
        }
        return 0;
    }

    public float FunnelIncreaseRange()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.FunnelIncreaseRange)
            {
                return _currentBuffs[i]._buffCount * _funnelRangeMultiplier;
            }
        }
        return 0;
    }

    public float FunnelIncreaseDamage()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.FunnelIncreaseDamage)
            {
                return _currentBuffs[i]._buffCount * _funnelDamageMultiplier;
            }
        }
        return 0;
    }

    public int FunnelIncreaseNumber()
    {
        for (int i = 0; i < _currentBuffs.Count; i++)
        {
            if (_currentBuffs[i]._buffName == BuffTypes.FunnelIncreaseNumber)
            {
                return _currentBuffs[i]._buffCount * _funnelNumberBase;
            }
        }
        return 0;
    }



    public static CharacterBuffs PInstance { get { return _instance; } }
    public List<Buffs> PCurrentBuffs { get { return _currentBuffs; } set { _currentBuffs = value; } }
}
