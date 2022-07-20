using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int _damageToPlayer;

    private void Start()
    {
        Destroy(this.gameObject, .5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            other.GetComponent<CharacterHealth>().ReduceHP(_damageToPlayer);
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
