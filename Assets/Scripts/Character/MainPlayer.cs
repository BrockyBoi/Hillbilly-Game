using System.Collections;
using System.Collections.Generic;
using Weaponry;
using UnityEngine;
using XP;

[RequireComponent(typeof(MainPlayerMovementComponent))]
[RequireComponent(typeof(PlayerAttributesComponent))]
[RequireComponent(typeof(PlayerXPComponent))]
[RequireComponent(typeof(CircleCollider2D))]
public class MainPlayer : Character
{
    public static MainPlayer Instance;

    private PlayerAttributesComponent _attributes;
    public PlayerAttributesComponent AttributesComponent { get { return _attributes; } }

    private PlayerXPComponent _playerXP;
    public PlayerXPComponent PlayerXPComponent { get { return _playerXP; } }

    private CircleCollider2D _xpOrbCollider;

    float _startingXPOrbColliderRadius;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        _attributes = GetComponent<PlayerAttributesComponent>();
        _playerXP = GetComponent<PlayerXPComponent>();
        _xpOrbCollider = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        AttributesComponent.OnAttributeChanged -= OnAttributeChanged;
        AttributesComponent.OnAttributeChanged += OnAttributeChanged;
    }

    private void OnDisable()
    {
        AttributesComponent.OnAttributeChanged -= OnAttributeChanged;
    }

    private void Start()
    {
        _startingXPOrbColliderRadius = _xpOrbCollider.radius;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        XPOrb orb = other.gameObject.GetComponent<XPOrb>();
        if (orb)
        {
            orb.OnEnterPlayerPickupRange();
        }
    }

    private void OnAttributeChanged(UpgradeAttribute attribute, float value)
    {
        if (attribute == UpgradeAttribute.PickupRange)
        {
            _xpOrbCollider.radius = _startingXPOrbColliderRadius * value;
        }
    }
}
