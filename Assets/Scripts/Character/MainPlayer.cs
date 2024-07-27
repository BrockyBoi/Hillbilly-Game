using System.Collections;
using System.Collections.Generic;
using Weaponry;
using UnityEngine;
using XP;

[RequireComponent(typeof(MainPlayerMovementComponent))]
[RequireComponent(typeof(PlayerArsenalComponent))]
[RequireComponent(typeof(PlayerXPComponent))]
public class MainPlayer : Character
{
    public static MainPlayer Instance;

    PlayerArsenalComponent _arsenal;
    public PlayerArsenalComponent ArsenalComponent { get { return _arsenal; } }

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        _arsenal = GetComponent<PlayerArsenalComponent>();
    }

    protected override void Update()
    {

    }
}
