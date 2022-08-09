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
    [SerializeField] private GameObject _gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
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
        if(_currentHp <= 0)
        {
            Time.timeScale = 0;
            _gameOverScreen.SetActive(true);
        }
    }

    public void AddHP(float addHP)
    {
        if ((_currentHp + (int)addHP) <= _maxHp)
            _currentHp += (int)addHP;
        else
            _currentHp = _maxHp;
        UpdateHP();
    }
}
