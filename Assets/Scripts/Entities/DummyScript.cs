using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : AEnemy
{
    public override void PathFinding()
    {
        intentionAttaque = false;
        nextMove = Vector2.zero;
    }

    public new void Action()
    {
        gameMaster.NextSkeleton();
    }

    protected override void Attack()
    {
        //Lol
    }

    protected override void PlayAttack()
    {
        audioManager.Play("DummyKilled");
    }

    protected override void PlayMovement()
    {
        audioManager.Play("DummyKilled");
    }

    protected override void PlayDeath()
    {
        audioManager.Play("DummyKilled");
    }

    protected override void ChangeSprite()
    {
       //Lol
    }
}
