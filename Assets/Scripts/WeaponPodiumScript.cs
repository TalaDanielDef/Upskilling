using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPodiumScript : MonoBehaviour
{
    [SerializeField] private GameObject _pressEGameObject;
    [SerializeField] private WeaponSO _weaponToChange;
    private bool _isPlayerNear;
    private GameObject _player;
    private CharacterCombat.WeaponTypes _weaponType;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(_isPlayerNear)
        {
            _weaponType = _player.GetComponent<CharacterCombat>().PCurrentWeaponSO._weaponType;
            if(_weaponType != _weaponToChange._weaponType)
                _pressEGameObject.SetActive(true);

        }
        else
        {
            _pressEGameObject.SetActive(false);
        }

        if(_isPlayerNear && Input.GetKeyDown(KeyCode.E) && _pressEGameObject.activeSelf)
        {
            CharacterCombat.PInstance.PCurrentWeaponSO = _weaponToChange;
            _player.GetComponent<CharacterCombat>().SwapWeapon();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            _isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            _isPlayerNear = false;
        }
    }
}
