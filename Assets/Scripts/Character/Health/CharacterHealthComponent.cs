using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthComponent : MonoBehaviour
{
    public bool GOD_MODE = false;

    [SerializeField]
    private float _defaultHealth = 100f;
    public float DefaultHealth { get { return _defaultHealth; } }

    private float _maxHealth;
    public float MaxHealth { get { return _maxHealth; } }

    private float _currentHealth = 100;

    public delegate void EOnKilled(Character characterKilled);
    public event EOnKilled OnKilled;

    public delegate void EOnHealthChange(float health);
    public event EOnHealthChange OnHealthChange;

    // Start is called before the first frame update
    void Start()
    {
        SetMaxHealth(_defaultHealth);
        SetHealth(_defaultHealth);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool DoesDamageKill(float damage)
    {
        return _currentHealth <= damage;
    }

    public void AddHealth(float healthToAdd)
    {
        if (healthToAdd > 0)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + healthToAdd, 0, _maxHealth);
            OnHealthChange?.Invoke(_currentHealth);
        }
    }

    public void SetHealth(float newHealth)
    {
        _currentHealth = newHealth;
        OnHealthChange?.Invoke(_currentHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public void DoDamage(float damage)
    {
        if (IsAlive() && damage > 0 && !GOD_MODE)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            OnHealthChange?.Invoke(_currentHealth);
            if (_currentHealth <= 0 )
            {
                Die();
            }
        }
    }

    protected virtual void Die()
    {
        OnKilled?.Invoke(GetOwningCharacter());
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public bool IsAlive()
    {
        return _currentHealth > 0;
    }

    protected Character GetOwningCharacter()
    {
        return GetComponent<Character>();
    }
}
