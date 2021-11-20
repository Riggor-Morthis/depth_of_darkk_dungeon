using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    //Private
    int coordX, coordY; //Les coordonnees de notre tile
    Color colour; //La couleur de notre tile
    SpriteRenderer spriteRenderer; //Le component du sprite

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

        transform.position = new Vector2(coordX, coordY);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colour;
        spriteRenderer.sortingOrder = -(100 + coordY);
    }
}
