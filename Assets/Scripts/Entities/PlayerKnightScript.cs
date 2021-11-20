using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnightScript : MonoBehaviour
{
    //Private
    Vector2 playerToMouse; //Le vecteur allant de la position actuelle du joueur a la position actuelle de la souris
    float touchAngle; //L'angle entre les "pieds" du joueur et l'endroit ou on a touche
    Vector3 moveVector; //Le vector du mouvement que l'on espere realiser

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Move();
        }
    }

    /// <summary>
    /// Assure le mouvement de notre joueur en fonction de la position du touch
    /// </summary>
    private void Move()
    {
        playerToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        touchAngle = Vector2.SignedAngle(transform.right, playerToMouse);
        
        //On determine le vecteur a creer
        if (Mathf.Abs(touchAngle) <= 45) moveVector = Vector3.right; //Si on est vers la droite du joueur, on va a droite
        else if (Mathf.Abs(touchAngle) >= 135) moveVector = Vector3.left; //Si on est vers la gauche du joueur, on va a gauche
        else if (touchAngle > 0) moveVector = Vector3.up; //Si on est au dessus du joueur, on va vers le haut
        else moveVector = Vector3.down; //Si on est en dessous du joueur, on va vers le bas

        //On applique notre mouvement prevu
        transform.position += moveVector;
    }
}
