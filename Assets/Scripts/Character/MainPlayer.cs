using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MainPlayerMovementComponent))]
[RequireComponent (typeof(PlayerArsenalComponent))]
public class MainPlayer : Character
{
    public static MainPlayer Instance;

    PlayerArsenalComponent _arsenal;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        _arsenal = GetComponent<PlayerArsenalComponent>();
    }

    protected override void Update()
    {
        if (!IsAlive())
        {
            Debug.Log("Dead");
        }
    }
}
