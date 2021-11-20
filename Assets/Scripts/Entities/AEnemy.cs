using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEnemy : AEntity
{
    public override void getHurt()
    {
        gameMaster.EnemyRemover(gameObject);
        gameObject.SetActive(false);
    }
}
