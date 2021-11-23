using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour
{
    //Public
    public Sprite[] spriteArray; //La ou on va stocker tous les sprites de l'entite

    //Protected
    protected GameMasterScript gameMaster; //Le maitre du jeu
    protected SpriteRenderer spriteRenderer; //Le component du sprite
    protected float actionSpeed; //Utilisee pour les animations
    protected AudioManagerScript audioManager; //Utilise pour tous les sons

    /// <summary>
    /// Modifie le rendering order de l'entite en fonction de ses coordonnes en y
    /// </summary>
    protected void CheckRenderingOrder()
    {
        spriteRenderer.sortingOrder = -(int)transform.position.y;
    }

    /// <summary>
    /// Cree l'entite de la bonne face
    /// </summary>
    /// <param name="start">position de depart du joueur</param>
    virtual public void Initialize(Vector2 start)
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMasterScript>();
        transform.position = start;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = spriteArray[0];
        CheckRenderingOrder();
        actionSpeed = 5f;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    /// <summary>
    /// Ce qu'il se passe lorsqu'on se fait toucher
    /// </summary>
    public abstract void getHurt(Vector2 attackDirection);
}
