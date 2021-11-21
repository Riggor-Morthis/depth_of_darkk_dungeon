using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : AEntity
{
    /// <summary>
    /// Ce qu'il se passe lorsque le monstre est attaque
    /// </summary>
    public override void getHurt(Vector2 attackDirection)
    {
        gameMaster.EnemyRemover(gameObject);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public abstract void PathFinding();

    /// <summary>
    /// Ordonne au monstre de faire l'action qu'il avait prevu
    /// </summary>
    public abstract void Action();
}
