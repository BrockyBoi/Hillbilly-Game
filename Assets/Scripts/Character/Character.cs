using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHealthComponent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Character : MonoBehaviour
{
    protected CharacterHealthComponent _healthComponent;
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

    protected virtual void OnCollisionEnter2D(Collision2D other) { }
    protected virtual void OnCollisionExit2D(Collision2D other) { }
    protected virtual void OnCollisionStay2D(Collision2D other) { }

    public bool IsAlive()
    {
        return GetHealthComponent().IsAlive();
    }

    public CharacterHealthComponent GetHealthComponent() 
    { 
        return _healthComponent; 
    }

}
