using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnightScript : MonoBehaviour
{
    //Public
    public GameMasterScript gameMaster;

    //Private
    Vector2 playerToMouse; //Le vecteur allant de la position actuelle du joueur a la position actuelle de la souris
    float touchAngle; //L'angle entre les "pieds" du joueur et l'endroit ou on a touche
    Vector3 moveVector; //Le vecteur de notre mouvement
    SpriteRenderer spriteRenderer; //Le component du sprite

    //On recupere la ou le joueur appuie, et on interprete alors l'ordre correspondant
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        if (gameMaster.AuthorizeMovement(transform.position + moveVector))
        {
            transform.position += moveVector;
            CheckRenderingOrder();
            gameMaster.EndLevelChecker();
        }
    }

    /// <summary>
    /// Modifie le rendering order du joueur en fonction de ses coordonnes en y
    /// </summary>
    private void CheckRenderingOrder()
    {
        spriteRenderer.sortingOrder = -(int)transform.position.y;
    }

    /// <summary>
    /// Cree le joueur de la bonne face
    /// </summary>
    /// <param name="start">position de depart du joueur</param>
    public void Initialize(Vector2 start)
    {
        transform.position = start;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CheckRenderingOrder();   
    }
}
