using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnightScript : AEntity
{
    //Private
    bool InputPossible = true; //Indiquer si les inputs joueurs sont actuellement autorises ou non
    Vector2 playerToMouse; //Le vecteur allant de la position actuelle du joueur a la position actuelle de la souris
    float touchAngle; //L'angle entre les "pieds" du joueur et l'endroit ou on a touche
    Vector3 moveVector; //Le vecteur de notre mouvement
    int isMovementPossible; //Pour stocker la reponse du game master par rapport au mouvement demande
    GameObject target; //L'ennemi qu'on essaye de toucher(si il y en a un)

    //On recupere la ou le joueur appuie, et on interprete alors l'ordre correspondant
    private void Update()
    {
        if (InputPossible && Input.GetMouseButtonDown(0))
        {
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
            gameMaster.EndLevelChecker();
        }
        if (isMovementPossible > 0)
        {
            target = gameMaster.GetEnemy(transform.position + moveVector);
            if (target != null) target.GetComponent<AEnemy>().getHurt();
        }
    }

    public override void getHurt()
    {
        //Rien pour le moment
    }
}
