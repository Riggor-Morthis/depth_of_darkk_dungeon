using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSkeletonScript : AEnemy
{
    //Private
    int inspector; //Utilise pour tracer des lignes ou des colonnes
    int shtong; //Utilise pour indiquer que le projectile a atteint quelque chose

    /// <summary>
    /// La fonction qui gere l'attaque de notre squelette
    /// </summary>
    protected override void Attack()
    {
        inspector = 1;
        shtong = 0;
        //On s'arrete si on trouve un mur, ou un ennemi
        while (shtong != -1 && shtong != 2)
        {
            //On regarde la cible actuelle
            shtong = gameMaster.AuthorizeMovement((Vector2)transform.position + inspector * nextMove);
            //Si la cible est une entite, on peut taper
            if (shtong == 2)
            {
                target = gameMaster.GetEntity((Vector2)transform.position + inspector * nextMove);
                target.GetComponent<AEntity>().getHurt(nextMove);
            }
            inspector++;
        }
    }

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
    /// La fonction qui joue le clip d'attaque
    /// </summary>
    protected override void PlayAttack()
    {
        audioManager.Play("MagicAttack");
    }

    /// <summary>
    /// La fonction qui joue le clip de pas
    /// </summary>
    protected override void PlayMovement()
    {
        audioManager.Play("Step");
    }

    /// <summary>
    /// La fonction qui joue le clip de mort
    /// </summary>
    protected override void PlayDeath()
    {
        audioManager.Play("SkeletonKilled");
    }

    /// <summary>
    /// La fonction qui modifie notre sprite en fonction de ce qu'on va faire
    /// </summary>
    protected override void ChangeSprite()
    {
        //On commence par determiner le meilleur sprite en fonction de ce qu'on va faire
        if (intentionAttaque) spriteY = 4;
        else spriteY = 0;
        if (nextMove == Vector2.down) spriteX = 0;
        else if (nextMove == Vector2.left) spriteX = 1;
        else if (nextMove == Vector2.up) spriteX = 2;
        else spriteX = 3;
        //Ensuite, on applique ce sprite
        spriteRenderer.sprite = spriteArray[spriteX + spriteY];
    }
}
