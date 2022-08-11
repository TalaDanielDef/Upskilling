using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemScript : MonoBehaviour
{
    [SerializeField] CharacterBuffs.BuffTypes _buffType;

    private CharacterBuffs _playerBuffs;
    private bool _isAdded = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            _playerBuffs = other.GetComponent<CharacterBuffs>();
            
            if(_playerBuffs.PCurrentBuffs.Count != 0)
            {
                for(int i = 0; i < _playerBuffs.PCurrentBuffs.Count; i++)
                {
                    if(_playerBuffs.PCurrentBuffs[i]._buffName == _buffType)
                    {
                        _playerBuffs.PCurrentBuffs[i]._buffCount++;
                        _isAdded = true;
                        break;
                    }
                }
            }
            if(!_isAdded)
            {
                _playerBuffs.PCurrentBuffs.Add(new CharacterBuffs.Buffs { _buffName = _buffType, _buffCount = 1 });
                _isAdded = true;
            }

            Destroy(this.gameObject);
        }
    }

    public CharacterBuffs.BuffTypes PPlayerBuff { get { return _buffType; } }
}
