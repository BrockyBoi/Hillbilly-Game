using StatusEffects;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XP;

[RequireComponent(typeof(CharacterHealthComponent))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CharacterStatusEffectsManager))]
public abstract class Character : MonoBehaviour
{
    protected CharacterHealthComponent _healthComponent;
    public CharacterHealthComponent HealthComponent { get { return _healthComponent; } }
    protected CharacterMovementComponent _characterMovementComponent;
    public CharacterMovementComponent CharacterMovementComponent { get { return _characterMovementComponent; } }
    [SerializeField]
    protected SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    protected BoxCollider2D _boxCollider;

    protected CharacterStatusEffectsManager _statusEffectsManager;
    public CharacterStatusEffectsManager StatusEffectsManager { get { return _statusEffectsManager; } }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _healthComponent = GetComponent<CharacterHealthComponent>();
        _characterMovementComponent = GetComponent<CharacterMovementComponent>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _statusEffectsManager = GetComponent<CharacterStatusEffectsManager>();

        if (_healthComponent == null)
        {
            Debug.LogError("Health Component is null " + gameObject.name);
        }

        if (_characterMovementComponent == null)
        {
            Debug.LogError("Character movement is null " + gameObject.name);
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer is null " + gameObject.name);
        }

        if (_boxCollider == null)
        {
            Debug.LogError("Box Collider is null " + gameObject.name);
        }
    }

    protected virtual void OnEnable()
    {
        _healthComponent.OnHealthChange -= OnHealthChange;
        _healthComponent.OnHealthChange += OnHealthChange;
    }

    protected virtual void OnDisable()
    {
        _healthComponent.OnHealthChange -= OnHealthChange;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) { }
    protected virtual void OnTriggerExit2D(Collider2D other) { }
    protected virtual void OnTriggerStay2D(Collider2D other) { }

    protected virtual void OnHealthChange(float currentHealth) { }

    public bool IsAlive()
    {
        return HealthComponent ? HealthComponent.IsAlive() : false;
    }

    public bool IsFrozen()
    {
        return _statusEffectsManager.GetStacks(EStatusEffectType.Freeze) > 0;
    }
}
