using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour
{
    //Protected
    protected GameMasterScript gameMaster;

    //Private
    SpriteRenderer spriteRenderer; //Le component du sprite

    /// <summary>
    /// Modifie le rendering order du joueur en fonction de ses coordonnes en y
    /// </summary>
    protected void CheckRenderingOrder()
    {
        spriteRenderer.sortingOrder = -(int)transform.position.y;
    }

    /// <summary>
    /// Cree le joueur de la bonne face
    /// </summary>
    /// <param name="start">position de depart du joueur</param>
    public void Initialize(Vector2 start)
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMasterScript>();
        transform.position = start;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CheckRenderingOrder();
    }

    /// <summary>
    /// Ce qu'il se passe lorsqu'on se fait toucher
    /// </summary>
    public abstract void getHurt();
}
