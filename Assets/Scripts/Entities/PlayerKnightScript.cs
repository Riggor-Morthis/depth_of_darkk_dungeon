using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnightScript : AEntity
{
    //Public
    public GameObject HelmetPrefab; //Le prefab de notre chapeau

    //Private
    bool inputPossible = false; //Indiquer si les inputs joueurs sont actuellement autorises ou non
    Vector2 playerToMouse; //Le vecteur allant de la position actuelle du joueur a la position actuelle de la souris
    float touchAngle; //L'angle entre les "pieds" du joueur et l'endroit ou on a touche
    Vector3 moveVector; //Le vecteur de notre mouvement
    int isMovementPossible; //Pour stocker la reponse du game master par rapport au mouvement demande
    GameObject target; //L'ennemi qu'on essaye de toucher(si il y en a un)
    bool helmet; //Est-ce que le joueur possede un Heaume ou non. Si il a un Heaume, il ne prend pas de degats mais il perd le Heaume
    int spriteX, spriteY; //Utilise pour connaitre les "coordonnees" du sprite a afficher
    bool acting; //Utilise pour indiquer que le joueur est en train d'agir
    Vector3 targetDestination; //La ou on veut aller
    float timer; //Pour passer le temps lorsqu'on attaque
    int score; //Le score qu'on accumule durant le niveau
    GameObject currentHelmet; //Le dernier heaume qu'on ait jete

    /// <summary>
    /// Cree l'entite de la bonne face
    /// </summary>
    /// <param name="start">position de depart du joueur</param>
    override public void Initialize(Vector2 start)
    {
        base.Initialize(start);
        helmet = true;
        score = 0;
    }

    //On recupere la ou le joueur appuie, et on interprete alors l'ordre correspondant
    private void Update()
    {
        //Si l'input est possible, on ecoute ce que le joueur a dire, puis on reagit en consequence
        if (inputPossible && Input.GetMouseButtonDown(0))
        {
            inputPossible = false;
            GetInput();
            MoveVectorCreator();
            MovePlayer();
        }
        //Sinon, si on peut agir on agit
        else if (acting)
        {
            //Tant qu'on est pas arrive, on fait que ca
            if(transform.position != targetDestination)
            {
                //Une fois arrive la ou on veut, il y a des verifications a faire
                if (Vector3.Distance(transform.position, targetDestination) <= 0.1f)
                {
                    //On arrondit notre position
                    transform.position = targetDestination;
                    CheckRenderingOrder();
                    //On recupere le mouvement droit devant nous
                    isMovementPossible = gameMaster.AuthorizeMovement(transform.position + moveVector);
                    //Si on a un 2, il faut lancer une phase d'attaque
                    if (isMovementPossible == 2)
                    {
                        audioManager.Play("SwordAttack");
                        //On recupere la cible et on la blesse
                        target = gameMaster.GetEntity(transform.position + moveVector);
                        if (target != null) target.GetComponent<AEntity>().getHurt(moveVector);
                        //On change notre score
                        if (target.GetComponent<RangedSkeletonScript>() != null) ScoreModifier(10000);
                        else ScoreModifier(5000);
                        //On demarre le timer
                        timer = 0.20f;
                    }
                    //Sinon, on met le timer a 0 pour esquiver tout ca
                    else timer = 0;
                }
                else transform.position += (Vector3)moveVector * actionSpeed * Time.deltaTime;
                
            }
            //Sinon, on laisse le temps passer
            else if(timer <= 0)
            {
                acting = false;
                gameMaster.NewLoop();
            }
            else timer -= Time.deltaTime;
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
        //On demande au maitre du jeu si le mouvement est possible
        isMovementPossible = gameMaster.AuthorizeMovement(transform.position + moveVector);
        //Si il est possible on commence les animations
        if (isMovementPossible > 0)
        {
            ChangeSprite();
            acting = true;
            //Si on a un ennemi devant
            if (isMovementPossible == 2)
            {
                audioManager.Play("SwordAttack");
                //On reste ici
                targetDestination = transform.position;
                //On recupere la cible et on la blesse
                target = gameMaster.GetEntity(transform.position + moveVector);
                if (target != null) target.GetComponent<AEntity>().getHurt(moveVector);
                //On change notre score
                if (target.GetComponent<RangedSkeletonScript>() != null) ScoreModifier(10000);
                else ScoreModifier(5000);
                //On demarre le timer
                timer = 0.20f;
            }
            else
            {
                audioManager.Play("Step");
                targetDestination = transform.position + moveVector;
                ScoreModifier(-247);
            }
        }
        //Sinon, on re-autorise les inputs du joueur
        else inputPossible = true;
    }

    /// <summary>
    /// La fonction qui s'assure qu'on ait le bon sprite a tout moment
    /// </summary>
    void ChangeSprite()
    {
        //On commence par determiner le meilleur sprite en fonction de ce qu'on va faire
        if (helmet) spriteY = 0;
        else spriteY = 4;
        if (moveVector == Vector3.down) spriteX = 0;
        else if (moveVector == Vector3.left) spriteX = 1;
        else if (moveVector == Vector3.up) spriteX = 2;
        else spriteX = 3;
        //Ensuite, on applique ce sprite
        spriteRenderer.sprite = spriteArray[spriteX + spriteY];
    }


    void ScoreModifier(int s)
    {
        score = Mathf.Max(0, score + s);
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
        if (!(attackDirection == -(Vector2)moveVector))
        {
            //Si on a un Heaume, pas de degats
            if (helmet)
            {
                audioManager.Play("HelmetDestroyed");
                currentHelmet = GameObject.Instantiate(HelmetPrefab);
                currentHelmet.GetComponent<ParticleScript>().Initialize(transform.position);

                helmet = false;
                ChangeSprite();
                ScoreModifier(-2500);
            }
            //Sinon, on a perdu
            else
            {
                audioManager.Play("PlayerKilled");
                gameMaster.ReloadScene();
            }
        }
        else audioManager.Play("ShieldBlock");
    }

    /// <summary>
    /// Indique au joueur qu'il vient de recuperer un tresor
    /// </summary>
    public void ScoreTreasure()
    {
        if (!helmet)
        {
            helmet = true;
            ChangeSprite();
            ScoreModifier(6500);
        }
        else ScoreModifier(13000);
    }

    /// <summary>
    /// Give the player's scores
    /// </summary>
    /// <returns>le score actuel du joueur</returns>
    public int getScore() => score;
}