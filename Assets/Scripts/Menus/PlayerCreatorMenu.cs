using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weaponry;

public class PlayerCreatorMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _weaponsChoices;

    PlayerLoadout _playerLoadout;

    bool _isChoosingWeapon = true;


    void Start()
    {
        _playerLoadout = PlayerLoadout.Instance;

        SetWeaponsData();
    }

    void SetWeaponsData()
    {
        int count = 1;
        _weaponsChoices.text = string.Empty;
        foreach(WeaponScriptableObject weapon in WeaponsDataManager.Instance.AllWeaponsData)
        {
            _weaponsChoices.text += count.ToString() + ". " + weapon.WeaponData.WeaponID + "\n";
            count++;
        }
    }

    void SetTraitsData()
    {
        int count = 1;
        _weaponsChoices.text = string.Empty;
        foreach (CharacterTraitScriptableObject trait in TraitsDataManager.Instance.AllCharacterTraitsData)
        {
            _weaponsChoices.text += count.ToString() + ". " + trait.TraitName + "\n";
            count++;
        }
    }

    void ChooseData(int dataNum)
    {
        if (_isChoosingWeapon && WeaponsDataManager.Instance.AllWeaponsData.Count > dataNum)
        {
            _playerLoadout.ToggleWeapon(WeaponsDataManager.Instance.AllWeaponsData[dataNum].WeaponData);
        }
        else if (!_isChoosingWeapon && TraitsDataManager.Instance.AllCharacterTraitsData.Count > dataNum)
        {
            _playerLoadout.ToggleTrait(TraitsDataManager.Instance.AllCharacterTraitsData[dataNum].Trait);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isChoosingWeapon = !_isChoosingWeapon;

            if(_isChoosingWeapon)
            {
                SetWeaponsData();
            }
            else
            {
                SetTraitsData();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChooseData(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChooseData(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChooseData(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChooseData(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChooseData(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChooseData(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChooseData(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ChooseData(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ChooseData(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChooseData(9);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("DefaultZoo");
        }
    }
}
