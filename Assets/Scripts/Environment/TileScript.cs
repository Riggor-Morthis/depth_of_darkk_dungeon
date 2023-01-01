using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    //Private
    int coordX, coordY; //Les coordonnees de notre tile
    Color colour; //La couleur de notre tile
    Color tempColor; //La couleur de notre tile lorsqu'un ennemi declare ses intentions dessus
    SpriteRenderer spriteRenderer; //Le component du sprite
    bool altered; //Est-ce que la couleur n'est pas la couleur de base ?
    List<Vector2> voisins; //La liste des coordonnes des voisins
    int distance; //Distance actuelle au joueur (distance manhattan)

    /// <summary>
    /// Initialiser la tuile au bon endroit avec la bonne couleur
    /// </summary>
    /// <param name="X">coordonnees en x</param>
    /// <param name="Y">coordonnees en y</param>
    /// <param name="Colour">quelle couleur il faut leur donner</param>
    public void Initialize(int X, int Y, Color Colour)
    {
        coordX = X;
        coordY = Y;
        colour = Colour;
        altered = false;

        transform.position = new Vector2(coordX, coordY);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = colour;
        spriteRenderer.sortingOrder = -(100 + coordY);

        voisins = new List<Vector2>();
    }

    /// <summary>
    /// Exclusivement utilise par les tresors, pour s'afficher au dessus du sol
    /// </summary>
    public void ChaneSortingOrder()
    {
        spriteRenderer.sortingOrder += 2;
    }

    /// <summary>
    /// Change la couleur vers une nouvelle couleur pour faire interface
    /// </summary>
    /// <param name="color">la nouvelle couleur a mettre</param>
    public void ChangeColor(Color color)
    {
        if (!altered)
        {
            tempColor = color;
            altered = true;
        }
        //Une intention existe deja sur cette case, on va donc modifier la couleur de l'intention pour refleter les multiples intentions
        else
        {
            tempColor.b = Mathf.Max(0f, tempColor.b - 0.1f);
            //Plus de rouge que de vert ? C'est une attaque qui est declare
            if (color.r > color.g) tempColor.g = Mathf.Max(0f, tempColor.g - 0.1f);
            else tempColor.r = Mathf.Max(0f, tempColor.r - 0.1f);
        }
        spriteRenderer.color = tempColor;
    }

    /// <summary>
    /// Restore la couleur originelle de la tuile
    /// </summary>
    public void ResetColor()
    {
        spriteRenderer.color = colour;
        altered = false;
    }

    /// <summary>
    /// Rajoute un voisin a notre case
    /// </summary>
    /// <param name="coordonnees">Les coordonnees de grille de la case voisine</param>
    public void AddNeighbor(Vector2 coordonnees)
    {
        voisins.Add(coordonnees);
    }

    /// <summary>
    /// Indique si la case a vu sa couleur changer ou non
    /// </summary>
    /// <returns>True si la couleur n'est pas la couleur d'origine</returns>
    public bool getAltered() => altered;

    /// <summary>
    /// Donne une nouvelle valeur a la distance manhattan de la case par rapport au joueur
    /// </summary>
    /// <param name="d">La distance entiere separant la case du joueur</param>
    public void setDistance(int d)
    {
        distance = d;
    }

    /// <summary>
    /// Obtient la distance manhattan de la case par rapport au joueur
    /// </summary>
    /// <returns>La distance manhattan de la case par rapport au joueur</returns>
    public int getDistance() => distance;

    /// <summary>
    /// Retourne la liste des voisins de notre case actuelle
    /// </summary>
    /// <returns>La liste des voisins de notre case actuelle</returns>
    public List<Vector2> getNeighbors() => voisins;
}
