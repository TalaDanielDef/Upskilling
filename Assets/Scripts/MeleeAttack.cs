using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int _damageToPlayer;
    private bool _haveDamaged = false;

    private void Start()
    {
        Destroy(this.gameObject, .5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            if(!_haveDamaged)
            {
                other.GetComponent<CharacterHealth>().ReduceHP(_damageToPlayer);
                _haveDamaged = true;
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag.Equals("Player"))
        {
            Debug.Log("Collided");
        }
    }
}
