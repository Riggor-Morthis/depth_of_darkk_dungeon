using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkeletonScript : AEnemy
{
    private bool moving = false;

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public override void PathFinding()
    {
        //On commence par récupérer la distance au joueur
        distanceJoueur = Vector3.Distance(transform.position, gameMaster.getPlayerPosition());
        //Si on est assez proche, on va vers le joueur directement
        if (distanceJoueur < 4)
        {
            //On commence par obtenir la meilleure case a atteindre
            nextMove = gameMaster.PathFinding((Vector2)transform.position);
            //On y soustrait notre position actuelle pour avoir un vrai vecteur de mouvement
            nextMove.x = Mathf.RoundToInt(nextMove.x - transform.position.x);
            nextMove.y = Mathf.RoundToInt(nextMove.y - transform.position.y);
            //Enfin, on determine nos intentations
            if (distanceJoueur <= 1f) intentionAttaque = true;
            else if (distanceJoueur <= 2f) if (Random.value <= 0.5f) intentionAttaque = true; else intentionAttaque = false;
            else if (distanceJoueur <= 3f) if (Random.value <= 0.25f) intentionAttaque = true; else intentionAttaque = false;
            else intentionAttaque = false;
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
        //On oublie pas de changer son sprite
        ChangeSprite();
    }

    /// <summary>
    /// Ordonne au monstre de faire l'action qu'il avait prevu
    /// </summary>
    public override void Action()
    {
        if (!intentionAttaque)
        {
            if (gameMaster.AuthorizeMovement((Vector2)transform.position + nextMove) == 1)
            {
                targetDestination = transform.position + (Vector3)nextMove;
                moving = true;
            }
        }
        else
        {
            target = gameMaster.GetEntity((Vector2)transform.position + nextMove);
            if (target != null) target.GetComponent<AEntity>().getHurt(nextMove);
        }
    }

    private void Update()
    {
        if (moving)
        {
            if (Vector3.Distance(transform.position, targetDestination) <= 0.05f)
            {
                moving = false;
                gameMaster.NextSkeleton();
            }
            else
            {
                transform.position += (Vector3)nextMove * actionSpeed * Time.deltaTime;
            }
        }
    }
}
