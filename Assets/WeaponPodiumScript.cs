using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPodiumScript : MonoBehaviour
{
    private bool _isPlayerNear;
    [SerializeField] private GameObject _pressEGameObject;
    [SerializeField] private GameObject _swordPodium, _bowPodium;
    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_isPlayerNear)
        {
            _pressEGameObject.SetActive(true);
        }
        else
        {
            _pressEGameObject.SetActive(false);
        }

        if(_isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if(_swordPodium.activeSelf)
            {
                _swordPodium.SetActive(false);
                _bowPodium.SetActive(true);
            }
            else if(_bowPodium.activeSelf)
            {
                _swordPodium.SetActive(true);
                _bowPodium.SetActive(false);
            }
            _player = GameObject.FindGameObjectWithTag("Player");
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
