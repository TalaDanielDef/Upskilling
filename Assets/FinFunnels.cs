using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinFunnels : MonoBehaviour
{
    [SerializeField] private float _finFunnelRange;
    [SerializeField] private int _damage;
    [SerializeField] private LineRenderer _lineRend;
    private GameObject[] _enemies;
    [SerializeField] private float _timeBtwnShots;
    [SerializeField] private GameObject _laserPosition;
    [SerializeField] private float _laserDuration;
    private float _timer;
    private bool _startLaserTimer;
    void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void Update()
    {
        if(_enemies.Length > 0)
        {
            transform.LookAt(_enemies[0].transform.GetChild(5).gameObject.transform.position);
            _timer += Time.deltaTime;
            if(_timeBtwnShots <= _timer)
            {
                _lineRend.SetPosition(0, _laserPosition.transform.position);
                _lineRend.SetPosition(1, _enemies[0].transform.GetChild(5).gameObject.transform.position);
                StartCoroutine(ShootLaser());
                _timer = 0;
            }

        }
    }

    IEnumerator ShootLaser()
    {
        _lineRend.enabled = true;
        yield return new WaitForSeconds(_laserDuration);
        _lineRend.enabled = false;
    }
}
