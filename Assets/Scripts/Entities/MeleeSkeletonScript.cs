using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkeletonScript : AEnemy
{
    //Private
    float distanceJoueur; //La distance qui nous separe du joueur
    Vector2 nextMove; //La ou l'ennemi va aller/attaquer la prochaine fois
    bool intentionAttaque; //Est-ce que le monstre compte nous attaquer, ou non ?
    GameObject target; //Le truc qu'on essaye de toucher

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public override void PathFinding()
    {
        //On commence par obtenir la meilleure case a atteindre
        nextMove = gameMaster.PathFinding((Vector2)transform.position);
        //On y soustrait notre position actuelle pour avoir un vrai vecteur de mouvement
        nextMove.x = Mathf.RoundToInt(nextMove.x - transform.position.x);
        nextMove.y = Mathf.RoundToInt(nextMove.y - transform.position.y);

        //Ensuite, on recupere la distance au joueur
        distanceJoueur = Vector3.Distance(transform.position, gameMaster.getPlayerPosition());

        //Fort de cette information, on l'utilise pour determiner une strategie
        if (distanceJoueur <= 1.1f) intentionAttaque = true;
        else if (distanceJoueur <= 2.1) if (Random.value <= 0.5f) intentionAttaque = true; else intentionAttaque = false;
        else if (distanceJoueur <= 3.1) if (Random.value <= 1 / 3f) intentionAttaque = true; else intentionAttaque = false;
        else intentionAttaque = false;

        //On indique nos intentions sur le plateau
        gameMaster.EnemyIntention((int)((Vector2)transform.position + nextMove).x, (int)((Vector2)transform.position + nextMove).y, intentionAttaque);
    }

    /// <summary>
    /// Ordonne au monstre de faire l'action qu'il avait prevu
    /// </summary>
    public override void Action()
    {
        if (!intentionAttaque)
        {
            if (gameMaster.AuthorizeMovement((Vector2)transform.position + nextMove) == 1) transform.position += (Vector3)nextMove;
        }
        else
        {
            target = gameMaster.GetEntity((Vector2)transform.position + nextMove);
            if (target != null) target.GetComponent<AEntity>().getHurt(nextMove);
        }
    }
}
