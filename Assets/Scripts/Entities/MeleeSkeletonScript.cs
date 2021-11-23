using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkeletonScript : AEnemy
{
    /// <summary>
    /// La fonction qui gere l'attaque de notre squelette
    /// </summary>
    protected override void Attack()
    {
        //On essaye de recuperer l'entite visee, et de lui faire des degats si elle existe
        target = gameMaster.GetEntity((Vector2)transform.position + nextMove);
        if (target != null) target.GetComponent<AEntity>().getHurt(nextMove);
    }

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
    /// La fonction qui joue le clip d'attaque
    /// </summary>
    protected override void PlayAttack()
    {
        audioManager.Play("SwordAttack");
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
