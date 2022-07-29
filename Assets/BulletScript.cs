using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour, IPooledObject
{
    private GameObject _player;
    private Rigidbody _rb;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _damageToPlayer;
    private bool _haveDamaged = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (!_haveDamaged)
            {
                other.GetComponent<CharacterHealth>().ReduceHP(_damageToPlayer);
                _haveDamaged = true;
                this.gameObject.SetActive(false);
            }
        }

        else if (other.tag.Equals("PlayerSword"))
        {
            if(_player.GetComponent<CharacterCombat>()._isAttacking)
            {
                this.gameObject.SetActive(false);
            }
        }

        else if (other.tag.Equals("Arrow"))
        {
            this.gameObject.SetActive(false);
        }

        else
        {
            this.gameObject.SetActive(false);
        }

        //wall SetActiveFalse
    }

    public void OnObjectSpawn()
    {
        _haveDamaged = false;
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
