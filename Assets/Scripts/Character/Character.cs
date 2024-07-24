using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterHealthComponent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Character : MonoBehaviour
{
    protected CharacterHealthComponent _healthComponent;
    public CharacterHealthComponent HealthComponent { get { return _healthComponent; } }
    protected CharacterMovementComponent _characterMovementComponent;
    protected SpriteRenderer _spriteRenderer;
    protected BoxCollider2D _boxCollider;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _healthComponent = GetComponent<CharacterHealthComponent>();
        _characterMovementComponent = GetComponent<CharacterMovementComponent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        if (_healthComponent == null)
        {
            Debug.LogError("Health Component is null");
        }

        if (_characterMovementComponent == null)
        {
            Debug.LogError("Character movement is null");
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer is null");
        }

        if (_boxCollider == null)
        {
            Debug.LogError("Box Collider is null");
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) { }
    protected virtual void OnTriggerExit2D(Collider2D other) { }
    protected virtual void OnTriggerStay2D(Collider2D other) { }

    public bool IsAlive()
    {
        return HealthComponent ? HealthComponent.IsAlive() : false;
    }
}