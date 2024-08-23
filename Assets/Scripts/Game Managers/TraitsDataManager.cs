using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

public class TraitsDataManager : MonoBehaviour
{
    public static TraitsDataManager Instance { get; private set; }

    [SerializeField]
    private List<CharacterTraitScriptableObject> _allCharacterTraitsData;

    public List<CharacterTraitScriptableObject> AllCharacterTraitsData { get { return _allCharacterTraitsData; } }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }
}
