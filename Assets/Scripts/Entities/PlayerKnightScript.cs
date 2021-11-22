using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnightScript : AEntity
{
    //Private
    bool inputPossible = false; //Indiquer si les inputs joueurs sont actuellement autorises ou non
    Vector2 playerToMouse; //Le vecteur allant de la position actuelle du joueur a la position actuelle de la souris
    float touchAngle; //L'angle entre les "pieds" du joueur et l'endroit ou on a touche
    Vector3 moveVector; //Le vecteur de notre mouvement
    int isMovementPossible; //Pour stocker la reponse du game master par rapport au mouvement demande
    GameObject target; //L'ennemi qu'on essaye de toucher(si il y en a un)
    bool Helmet; //Est-ce que le joueur possede un Heaume ou non. Si il a un Heaume, il ne prend pas de degats mais il perd le Heaume
    int spriteX, spriteY; //Utilise pour connaitre les "coordonnees" du sprite a afficher

    /// <summary>
    /// Cree l'entite de la bonne face
    /// </summary>
    /// <param name="start">position de depart du joueur</param>
    override public void Initialize(Vector2 start)
    {
        base.Initialize(start);
        Helmet = true;
    }

    //On recupere la ou le joueur appuie, et on interprete alors l'ordre correspondant
    private void Update()
    {
        if (inputPossible && Input.GetMouseButtonDown(0))
        {
            inputPossible = false;
            GetInput();
            MoveVectorCreator();
            MovePlayer();
        }
    }

    /// <summary>
    /// Recupere les inputs du joueur
    /// </summary>
    private void GetInput()
    {
        playerToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        touchAngle = Vector2.SignedAngle(transform.right, playerToMouse);
    }

    /// <summary>
    /// Creer le vecteur de mouvement demande
    /// </summary>
    private void MoveVectorCreator()
    {
        if (Mathf.Abs(touchAngle) <= 45) moveVector = Vector3.right; //Si on est vers la droite du joueur, on va a droite
        else if (Mathf.Abs(touchAngle) >= 135) moveVector = Vector3.left; //Si on est vers la gauche du joueur, on va a gauche
        else if (touchAngle > 0) moveVector = Vector3.up; //Si on est au dessus du joueur, on va vers le haut
        else moveVector = Vector3.down; //Si on est en dessous du joueur, on va vers le bas
    }

    /// <summary>
    /// Verifie la validite du mouvement puis l'applique
    /// </summary>
    private void MovePlayer()
    {
        isMovementPossible = gameMaster.AuthorizeMovement(transform.position + moveVector);
        if (isMovementPossible == 1)
        {
            transform.position += moveVector;
            CheckRenderingOrder();
        }
        if (isMovementPossible > 0)
        {
            target = gameMaster.GetEntity(transform.position + moveVector);
            if (target != null) target.GetComponent<AEntity>().getHurt(moveVector);
            ChangeSprite();
            gameMaster.NewLoop();
        }
        else inputPossible = true;
    }

    void ChangeSprite()
    {
        //On commence par determiner le meilleur sprite en fonction de ce qu'on va faire
        if (Helmet) spriteY = 0;
        else spriteY = 4;
        if (moveVector == Vector3.down) spriteX = 0;
        else if (moveVector == Vector3.left) spriteX = 1;
        else if (moveVector == Vector3.up) spriteX = 2;
        else spriteX = 3;
        //Ensuite, on applique ce sprite
        spriteRenderer.sprite = spriteArray[spriteX + spriteY];
    }

    /// <summary>
    /// Pour que le gamemaster nous donne la permission d'agir
    /// </summary>
    public void AllowMovement()
    {
        inputPossible = true;
    }

    /// <summary>
    /// Ce qu'il se passe lorsqu'on est attaque
    /// </summary>
    public override void getHurt(Vector2 attackDirection)
    {
        //Il ne faut que le monstre attaque de face si il veut nous faire des degats
        if(!(attackDirection == -(Vector2)moveVector))
        {
            //Si on a un Heaume, pas de degats
            if (Helmet)
            {
                Helmet = false;
                ChangeSprite();
            }
            //Sinon, on a perdu
            else gameMaster.ReloadScene();
        }
    }
}
