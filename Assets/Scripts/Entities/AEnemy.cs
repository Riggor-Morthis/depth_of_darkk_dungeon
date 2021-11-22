using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : AEntity
{
    //Protected
    protected Vector2 nextMove; //La ou l'ennemi va aller/attaquer la prochaine fois
    protected bool intentionAttaque; //Est-ce que le monstre compte nous attaquer, ou non ?
    protected GameObject target; //Le truc qu'on essaye de toucher
    protected float distanceJoueur; //La distance qui nous separe du joueur
    protected int randomChoice; //Utilise pour l'aleatoire du comportement du monstre
    protected Vector3 targetDestination; //Utilise pour aller vers l'endroit vise

    //Private
    int spriteX, spriteY; //Utilise pour connaitre les "coordonnees" du sprite a afficher

    /// <summary>
    /// La fonction qui modifie notre sprite en fonction de ce qu'on va faire
    /// </summary>
    protected void ChangeSprite()
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
