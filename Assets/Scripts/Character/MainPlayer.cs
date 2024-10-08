using System.Collections;
using System.Collections.Generic;
using Weaponry;
using UnityEngine;
using XP;

[RequireComponent(typeof(MainPlayerMovementComponent))]
[RequireComponent(typeof(PlayerWeaponsComponent))]
[RequireComponent(typeof(PlayerXPComponent))]
[RequireComponent(typeof(CircleCollider2D))]
public class MainPlayer : Character
{
    public static MainPlayer Instance;

    private PlayerWeaponsComponent _weaponsComponent;
    public PlayerWeaponsComponent WeaponsComponent { get { return _weaponsComponent; } }

    private CharacterAttributesManager _upgradesAttributesComponent = new CharacterAttributesManager();
    public CharacterAttributesManager UpgradeAttributesComponent { get { return _upgradesAttributesComponent; } }

    private PlayerXPComponent _playerXP;
    public PlayerXPComponent PlayerXPComponent { get { return _playerXP; } }

    private CircleCollider2D _xpOrbCollider;

    float _startingXPOrbColliderRadius;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        _weaponsComponent = GetComponent<PlayerWeaponsComponent>();
        _playerXP = GetComponent<PlayerXPComponent>();
        _xpOrbCollider = GetComponent<CircleCollider2D>();
    }

    protected override void OnEnable()
    {
        UpgradeAttributesComponent.OnAttributeChanged -= OnAttributeChanged;
        UpgradeAttributesComponent.OnAttributeChanged += OnAttributeChanged;
    }

    protected override void OnDisable()
    {
        UpgradeAttributesComponent.OnAttributeChanged -= OnAttributeChanged;
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
