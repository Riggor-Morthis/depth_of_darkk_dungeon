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
    int randomChoice; //Utilise pour l'aleatoire du comportement du monstre

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public override void PathFinding()
    {
        //On commence par récupérer la distance au joueur
        distanceJoueur = Vector3.Distance(transform.position, gameMaster.getPlayerPosition());
        //Si on est assez proche, on va vers le joueur directement
        if (distanceJoueur < 3)
        {
            //On commence par obtenir la meilleure case a atteindre
            nextMove = gameMaster.PathFinding((Vector2)transform.position);
            //On y soustrait notre position actuelle pour avoir un vrai vecteur de mouvement
            nextMove.x = Mathf.RoundToInt(nextMove.x - transform.position.x);
            nextMove.y = Mathf.RoundToInt(nextMove.y - transform.position.y);
            //Enfin, on determine nos intentations
            if (distanceJoueur <= 1f) intentionAttaque = true;
            else if (distanceJoueur <= 2f) if (Random.value <= 0.5f) intentionAttaque = true; else intentionAttaque = false;
            else if (Random.value <= 0.25f) intentionAttaque = true; else intentionAttaque = false;
        }

        //Sinon, on est trop loin, donc on se contente d'errer aleatoirement
        else
        {
            intentionAttaque = false;
            do
            {
                randomChoice = Random.Range(0, 4);
                if (randomChoice == 0) nextMove = Vector2.up;
                else if (randomChoice == 1) nextMove = Vector2.down;
                else if (randomChoice == 2) nextMove = Vector2.right;
                else nextMove = Vector2.left;
            } while (gameMaster.AuthorizeMovement((Vector2)transform.position + nextMove) != 1);
        }

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
