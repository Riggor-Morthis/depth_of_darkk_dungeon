using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : AEntity
{
    //Public
    public GameObject BonePrefab; //Le prefab de nos os

    //Protected
    protected Vector2 nextMove; //La ou l'ennemi va aller/attaquer la prochaine fois
    protected bool intentionAttaque; //Est-ce que le monstre compte nous attaquer, ou non ?
    protected GameObject target; //Le truc qu'on essaye de toucher
    protected float distanceJoueur; //La distance qui nous separe du joueur
    protected int randomChoice; //Utilise pour l'aleatoire du comportement du monstre
    protected Vector3 targetDestination; //Utilise pour aller vers l'endroit vise
    protected int spriteX, spriteY; //Utilise pour connaitre les "coordonnees" du sprite a afficher

    //Private
    bool acting = false; //Indique a l'entite qu'il est temps qu'elle agisse
    float timer; //Decompte le temps lorsque l'action est une attaque, qui se fait donc instantannement
    bool dying = false; //Lorsqu'on est en train de mourir, pour que le fade soit gere dans le update
    float currentAlpha; //Lorsqu'on est en train de mourir, c'est cet alpha qu'on decremenete
    float dyingSpeed; //La vitesse a laquelle on fade
    GameObject currentBone; //L'os qu'on est actuellement en train de jeter

    /// <summary>
    /// C'est ici qu'ont lieu les animations
    /// </summary>
    private void Update()
    {
        //Si on a le droit d'agir
        if (acting)
        {
            //Si on veut bouger, on y va pas a pas jusqu'a etre au bon endroit
            if (!intentionAttaque)
            {
                if (Vector3.Distance(transform.position, targetDestination) <= 0.1f)
                {
                    transform.position = targetDestination;
                    CheckRenderingOrder();
                    acting = false;
                    ChangeSprite();
                    gameMaster.NextSkeleton();
                }
                else transform.position += (Vector3)nextMove * actionSpeed * Time.deltaTime;
            }
            //Sinon, on decremente notre timer jusqu'a en avoir fini
            else
            {
                if (timer <= 0)
                {
                    acting = false;
                    gameMaster.NextSkeleton();
                }
                timer -= Time.deltaTime;
            }
        }
        if (dying)
        {
            if (currentAlpha <= 0) gameObject.SetActive(false);
            else
            {
                currentAlpha -= dyingSpeed * Time.deltaTime;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentAlpha / 255);
            }
        }
    }

    /// <summary>
    /// Ce qu'il se passe lorsque le monstre est attaque
    /// </summary>
    public override void getHurt(Vector2 attackDirection)
    {
        PlayDeath();
        //Si on a un prefab d'os, on l'utilise
        if(BonePrefab != null)
        {
            for(int i = 0; i < 3; i++)
            {
                currentBone = GameObject.Instantiate(BonePrefab);
                currentBone.GetComponent<ParticleScript>().Initialize(transform.position);
            }
        }

        gameMaster.EnemyRemover(gameObject);
        acting = false;
        dying = true;
        currentAlpha = 255;
        dyingSpeed = currentAlpha * 4;
    }

    /// <summary>
    /// Ordonne au monstre de faire l'action qu'il avait prevu
    /// </summary>
    public void Action()
    {
        //Si on compte pas attaquer, on trouve l'endroit qu'on veut
        if (!intentionAttaque)
        {
            if (gameMaster.AuthorizeMovement((Vector2)transform.position + nextMove) == 1)
            {
                targetDestination = transform.position + (Vector3)nextMove;
                PlayMovement();
            }
        }
        //Sinon, on attaque direct
        else
        {
            PlayAttack();
            Attack();
            timer = 0.20f;
        }
        acting = true;
    }

    /// <summary>
    /// Demande a l'entite d'attaquer
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// La fonction qui joue le clip d'attaque
    /// </summary>
    protected abstract void PlayAttack();

    /// <summary>
    /// La fonction qui joue le clip de pas
    /// </summary>
    protected abstract void PlayMovement();

    /// <summary>
    /// La fonction qui joue le clip de mort
    /// </summary>
    protected abstract void PlayDeath();

    /// <summary>
    /// La fonction qui modifie notre sprite en fonction de ce qu'on va faire
    /// </summary>
    protected abstract void ChangeSprite();

    /// <summary>
    /// Ordonne au monstre de trouver son prochain mouvement
    /// </summary>
    public abstract void PathFinding();
}
