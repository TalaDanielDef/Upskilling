using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private int _arrowDamage;
    private Rigidbody _rb;
    [SerializeField] private float _arrowSpeed;
    private float _distanceToDestroy;
    private Vector3 _offset;
    private GameObject _player;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        _rb.velocity = transform.forward * _arrowSpeed;
        float _distance = Vector3.Distance(this.transform.position, _player.transform.position);
        if(_distance > _distanceToDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Enemy"))
        {
            other.GetComponent<EnemyScript>().ReduceHP(_arrowDamage);
            Destroy(this.gameObject);
        }
    }

    public float PDistanceToDestroy { set { _distanceToDestroy = value; } }

}
