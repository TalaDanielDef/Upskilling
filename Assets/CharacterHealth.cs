using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int _maxHp;
    [SerializeField] private int _currentHp;
    [SerializeField] private Slider _hpSlider;
    // Start is called before the first frame update
    void Start()
    {
        _currentHp = _maxHp;
    }

    public void UpdateHP()
    {
        _hpSlider.value = (float)_currentHp / (float)_maxHp;
    }

    public void ReduceHP(int reduceHP)
    {
        _currentHp -= reduceHP;
        UpdateHP();
    }
}
