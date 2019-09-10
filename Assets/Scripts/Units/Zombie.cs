using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Unit
{
    public override void AfterBirth()
    {
        StartMoving();
    }

    protected override void OnAttack()
    {
    }

    protected override void OnDie()
    {
    }

    protected override void OnGetDamage()
    {
    }
}
