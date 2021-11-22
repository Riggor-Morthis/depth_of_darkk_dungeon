using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSkeletonScript : AEnemy
{
    //Private
    int inspector; //Utilise pour tracer des lignes ou des colonnes
    int shtong; //Utilise pour indiquer que le projectile a atteint quelque chose
    
    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public override void PathFinding()
    {
        //Si ce squelette est sur la meme ligne/colonne que le joueur, il choisit toujours d'attaquer
        if(gameMaster.getPlayerPosition().x == transform.position.x || gameMaster.getPlayerPosition().y == transform.position.y)
        {
            intentionAttaque = true;

            //On determine notre mouvement
            if(gameMaster.getPlayerPosition().x == transform.position.x)
            {
                if (gameMaster.getPlayerPosition().y - transform.position.y > 0) nextMove = Vector2.up;
                else nextMove = Vector2.down;
            }
            else
            {
                if (gameMaster.getPlayerPosition().x - transform.position.x > 0) nextMove = Vector2.right;
                else nextMove = Vector2.left;
            }

            //On declare nos intentions
            inspector = 1;
            while (gameMaster.AuthorizeMovement((Vector2)transform.position + inspector * nextMove) != -1) 
            {
                gameMaster.EnemyIntention((int)((Vector2)transform.position + inspector * nextMove).x, (int)((Vector2)transform.position + inspector * nextMove).y, intentionAttaque);
                inspector++;
            }
        }
        //Sinon, on choisit toujours de se deplacer
        else
        {
            intentionAttaque = false;
            //On commence par récupérer la distance au joueur
            distanceJoueur = Vector3.Distance(transform.position, gameMaster.getPlayerPosition());
            //Si on est assez proche, on va vers le joueur directement
            if (distanceJoueur < 4f)
            {
                //On commence par obtenir la meilleure case a atteindre
                nextMove = gameMaster.PathFinding((Vector2)transform.position);
                //On y soustrait notre position actuelle pour avoir un vrai vecteur de mouvement
                nextMove.x = Mathf.RoundToInt(nextMove.x - transform.position.x);
                nextMove.y = Mathf.RoundToInt(nextMove.y - transform.position.y);
            }

            //Sinon, on est trop loin, donc on se contente d'errer aleatoirement
            else
            {
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
            if (gameMaster.AuthorizeMovement((Vector2)transform.position + nextMove) == 1) transform.position += (Vector3)nextMove;
        }
        else
        {
            //On cherche ce sur quoi le tir s'arrete, entite ou mur du fond
            inspector = 1;
            shtong = 0;
            while (shtong != -1 && shtong != 2)
            {
                shtong = gameMaster.AuthorizeMovement((Vector2)transform.position + inspector * nextMove);
                if(shtong == 2)
                {
                    target = gameMaster.GetEntity((Vector2)transform.position + inspector * nextMove);
                    target.GetComponent<AEntity>().getHurt(nextMove);
                }
                inspector++;
            }
        }
        gameMaster.NextSkeleton();
    }
}
