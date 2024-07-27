using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthComponent : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth = 100f;
    private float _currentHealth = 100;

    public delegate void EOnKilled(Character characterKilled);
    public event EOnKilled OnKilled;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
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
        }
    }

    public void DoDamage(float damage)
    {
        if (IsAlive() && damage > 0)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);

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
