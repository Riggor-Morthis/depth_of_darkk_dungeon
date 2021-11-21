using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkeletonScript : AEnemy
{
    //Private
    Vector3 player; //La ou on stocke la position du joueur
    float distanceJoueur; //La distance qui nous separe du joueur
    Vector2 nextMove; //La ou l'ennemi va aller/attaquer la prochaine fois
    bool intentionAttaque; //Est-ce que le monstre compte nous attaquer, ou non ?

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public override void PathFinding()
    {
        player = gameMaster.getPlayerPosition();
        nextMove = player - transform.position;
        if(Mathf.Abs(nextMove.x) >= Mathf.Abs(nextMove.y)) //On a plus de mouvement en X donc on va en (+-1, 0)
        {
            if (nextMove.x > 0) nextMove = Vector2.right; //x positif
            else nextMove = Vector2.left; //x negatif
        }
        else
        {
            if (nextMove.y > 0) nextMove = Vector2.up;
            else nextMove = Vector2.down;
        }

        distanceJoueur = Vector3.Distance(transform.position, player);
        if (distanceJoueur <= 1.1f) intentionAttaque = true;
        else if (distanceJoueur <= 2.1) if (Random.value <= 0.5f) intentionAttaque = true; else intentionAttaque = false;
        else if (distanceJoueur <= 3.1) if (Random.value <= 1 / 3f) intentionAttaque = true; else intentionAttaque = false;
        else intentionAttaque = false;

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
            if (gameMaster.getPlayerPosition() == transform.position + (Vector3)nextMove) gameMaster.getPlayer().GetComponent<PlayerKnightScript>().getHurt(nextMove);
        }
    }
}
