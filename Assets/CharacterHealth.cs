using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int _maxHp;
    [SerializeField] private int _currentHp;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TextMeshProUGUI _hpText;
    // Start is called before the first frame update
    void Start()
    {
        _currentHp = _maxHp;
        UpdateHP();
    }

    public void UpdateHP()
    {
        _hpSlider.value = (float)_currentHp / (float)_maxHp;
        _hpText.text = _currentHp.ToString() + "/" + _maxHp.ToString();
    }

    public void ReduceHP(int reduceHP)
    {
        _currentHp -= reduceHP;
        UpdateHP();
    }
}
