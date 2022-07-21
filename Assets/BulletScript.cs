using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour, IPooledObject
{
    private GameObject _player;
    private Rigidbody _rb;
    [SerializeField] private float _bulletSpeed;
    public void OnObjectSpawn()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody>();

        Vector3 _moveDirection = (_player.transform.position - transform.position).normalized * _bulletSpeed;
        _rb.velocity = new Vector3(_moveDirection.x, _moveDirection.y, _moveDirection.z);
        StartCoroutine(SetActiveOff());
    }

    IEnumerator SetActiveOff()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
