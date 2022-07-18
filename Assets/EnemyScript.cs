using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _currentHp;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _player;
    private void Start()
    {
        _currentHp = _maxHp;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(_player.transform.position);
    }

    public void ReduceHP(int hpReduce)
    {
        _currentHp -= hpReduce;
        _hpBar.fillAmount = (float)_currentHp / (float)_maxHp;
    }
}
