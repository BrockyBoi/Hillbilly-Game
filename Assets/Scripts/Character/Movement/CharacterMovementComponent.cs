using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class CharacterMovementComponent : MonoBehaviour
{
    [SerializeField]
    protected float _movementSpeed = 5f;
    protected float _movementSpeedModifier = 1f;

    protected Character _owningCharacter;

    protected Vector2 _lastMovementVector = Vector2.zero;
    public Vector2 LastMovementVector { get { return _lastMovementVector; } }

    protected Vector2 _knockbackVector = Vector2.zero;
    protected bool _canKnockbackDamage = false;

    protected bool _isBeingKnockedBack = false;
    public bool IsBeingKnockedBack { get { return _isBeingKnockedBack; } }

    protected StatusEffectsManager _owningCharacterStatusEffectManager;

    private void Awake()
    {
        _owningCharacterStatusEffectManager = GetComponent<StatusEffectsManager>();

        _owningCharacter = GetComponent<Character>();
    }

    protected virtual void Start()
    {
        if (_owningCharacter)
        {
            _owningCharacter.HealthComponent.OnKilled -= OnCharacterKilled;
            _owningCharacter.HealthComponent.OnKilled += OnCharacterKilled;
        }
        else
        {
            Debug.LogError("Owning Character is null");
        }
    }

    protected virtual void OnDisable()
    {
        _owningCharacter.HealthComponent.OnKilled -= OnCharacterKilled;
    }

    private void OnCharacterKilled(Character characterKilled)
    {
        if (characterKilled == _owningCharacter)
        {
            StopAllCoroutines();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_owningCharacter && _owningCharacter.IsAlive() && !_owningCharacter.IsFrozen() && !_isBeingKnockedBack)
        {
            Move();
            RotateCharacter();
        }
    }

    protected abstract void Move();

    public void SetMovementSpeedModifier(float speedModifier)
    {
        _movementSpeedModifier = speedModifier;
    }

    public void AddKnockback(Vector3 knockbackDirection, bool canKnockbackDamage = false)
    {
        _knockbackVector = knockbackDirection;
        _canKnockbackDamage = canKnockbackDamage;

        if (_owningCharacter.IsAlive())
        {
            StartCoroutine(KnockBackCoroutine());
        }
    }

    protected IEnumerator KnockBackCoroutine()
    {
        _isBeingKnockedBack = true;

        float timeToKnockBack = .35f;
        float currentTime = 0;

        bool canKnockbackDamage = _canKnockbackDamage;

        Vector3 knockback = GetPotentialKnockback();
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + knockback;
        while (currentTime < timeToKnockBack)
        {
            currentTime += Time.deltaTime;

            Vector3 nextPos = Vector3.Lerp(startPos, endPos, currentTime / timeToKnockBack);
            if (canKnockbackDamage)
            {
                Vector2 dir = transform.position - nextPos;
                float dirMagnitude = dir.magnitude;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, dirMagnitude);
                foreach (RaycastHit2D hit in hits)
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                    if (enemy)
                    {
                        Vector3 enemyHitDir = transform.position - enemy.transform.position;
                        enemyHitDir.Normalize();
                        enemy.CharacterMovementComponent.AddKnockback(enemyHitDir * knockback.magnitude * .5f);
                    }
                }
            }

            transform.position = nextPos;

            yield return null;
        }

        _isBeingKnockedBack = false;
    }

    protected Vector2 GetPotentialKnockback()
    {
        Vector2 knockbackVector = Vector2.zero;
        if (_knockbackVector !=  Vector2.zero)
        {
            knockbackVector = _knockbackVector;
            _knockbackVector = Vector2.zero;
            _canKnockbackDamage = false;
        }

        return knockbackVector;
    }

    protected void RotateCharacter()
    {
        float angle = Vector3.Angle(_owningCharacter.transform.up, _lastMovementVector);
        if (_lastMovementVector.x > 0)
        {
            angle *= -1;
        }

        _owningCharacter.SpriteRenderer.transform.localRotation = Quaternion.Euler(0,0,angle);
    }
}
