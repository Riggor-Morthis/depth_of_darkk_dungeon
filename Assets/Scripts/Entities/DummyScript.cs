using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : AEnemy
{
    public override void PathFinding()
    {
        //Lol
    }

    public new void Action()
    {
        gameMaster.NextSkeleton();
    }

    protected override void Attack()
    {
        //Lol
    }
}
