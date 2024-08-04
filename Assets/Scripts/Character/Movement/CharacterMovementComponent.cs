using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMovementComponent : MonoBehaviour
{
    [SerializeField]
    protected float _movementSpeed = 5f;
    protected float _movementSpeedModifier = 1f;

    protected Character _owningCharacter;

    protected Vector2 _lastMovementVector = Vector2.zero;
    public Vector2 LastMovementVector { get { return _lastMovementVector; } }

    private void Awake()
    {
        _owningCharacter = GetComponent<Character>();

        if (_owningCharacter == null )
        {
            Debug.LogError("Owning Character is null");
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_owningCharacter && _owningCharacter.IsAlive())
        {
            Move();
        }
    }

    protected abstract void Move();

    public void SetMovementSpeedModifier(float speedModifier)
    {
        _movementSpeedModifier = speedModifier;
    }
}
