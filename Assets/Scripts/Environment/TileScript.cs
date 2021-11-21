using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    //Private
    int coordX, coordY; //Les coordonnees de notre tile
    Color colour; //La couleur de notre tile
    SpriteRenderer spriteRenderer; //Le component du sprite
    bool altered; //Est-ce que la couleur n'est pas la couleur de base ?

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

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colour;
        spriteRenderer.sortingOrder = -(100 + coordY);
    }

    /// <summary>
    /// Indique si la case a vu sa couleur changer ou non
    /// </summary>
    /// <returns>true si la couleur n'est pas la couleur d'origine</returns>
    public bool getAltered() => altered;

    /// <summary>
    /// Change la couleur vers une nouvelle couleur pour faire interface
    /// </summary>
    /// <param name="color">la nouvelle couleur a mettre</param>
    public void changeColor(Color color)
    {
        spriteRenderer.color = color;
        altered = true;
    }

    /// <summary>
    /// Restore la couleur originelle de la tuile
    /// </summary>
    public void resetColor()
    {
        spriteRenderer.color = colour;
        altered = false;
    }
}
