using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMasterScript : MonoBehaviour
{
    //Public
    public int levelWidth, levelHeight; //Les dimensions de notre dongeon en terme de nombre de tuiles
    public List<Vector2> holesInTheFloor; //La ou il n'y a pas de sol dans notre niveau
    public List<Vector2> enemyPositions; //La ou on va mettre nos ennemis
    public Vector2 playerPosition; //La position du joueur au debut du niveau
    public Vector2 endPosition; //La position de la tuile de fin de niveau
    public string nextLevel; //Le nom de la scene suivante
    public PlayerKnightScript playerKnight; //Le gameObject du joueur
    public GameObject tilePrefab; //Pour stocker le prefab de la tuile
    public GameObject enemyPrefab; //Pour stocker le prefab de l'ennemi

    //Private
    int startX, startY; //Point de depart des coordonnes (dans le negatif)
    GameObject[,] dungeonGrid; //Le sol du donjon, represente comme une grille
    Color[] colors = new Color[2] { new Color(173f / 255, 199f / 255, 204f / 255), new Color(145f / 255, 170f / 255, 194f / 255) }; //Les deux couleurs utilisees pour la quinquonce de notre damier
    Color endTile = new Color(161f / 255, 188f / 255, 201f / 255); //La couleur utilise par la tuile de fin de niveau
    Color[] mobIntentions = new Color[2] { new Color(108f / 255, 217f / 255, 126f / 255), new Color(217f / 255, 108f / 255, 126f / 255) }; //Les couleurs utilisees par "l'interface", respectivement deplacement puis attaque
    int pairImpair; //Juste pour savoir si on est sur une tuile paire ou impaire (initialisation uniquement)
    List<GameObject> enemies; // La liste de tous les ennemis actuellement presents dans le niveau


    /// <summary>
    /// Appeler au debut de la scene, pour tout initialiser
    /// </summary>
    void Start()
    {
        AspectRatioCalculator();
        Begin();
        FirstLoop();
    }

    /// <summary>
    /// Calcule la composante hauteur de la camera pour accueillir le niveau de maniere optimale
    /// </summary>
    void AspectRatioCalculator()
    {
        Camera.main.orthographicSize = Mathf.Max(levelHeight/2f + 0.2f, ((levelWidth / 2f + 0.2f) / Screen.width) * Screen.height);
    }

    /// <summary>
    /// La fonction utilise pour (re)initialiser la scence
    /// </summary>
    void Begin()
    {
        FloorBuilder();
        PlayerPlacer();
        EnemyPlacer();
    }

    /// <summary>
    /// Creer le sol du donjon grace a la grille de tuiles
    /// </summary>
    void FloorBuilder()
    {
        startX = -levelWidth / 2;
        startY = -levelHeight / 2;

        int tempX, tempY;
        dungeonGrid = new GameObject[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++) for (int j = 0; j < levelHeight; j++)
            {
                tempX = startX + i;
                tempY = startY + j;
                if(!holesInTheFloor.Contains(new Vector2(tempX, tempY)))
                {
                    dungeonGrid[i, j] = Instantiate(tilePrefab);
                    pairImpair = (i + j) % 2;
                    if (tempX == endPosition.x && tempY == endPosition.y) dungeonGrid[i, j].GetComponent<TileScript>().Initialize(tempX, tempY, endTile);
                    else dungeonGrid[i, j].GetComponent<TileScript>().Initialize(tempX, tempY, colors[pairImpair]);
                }
            }
    }

    /// <summary>
    /// Place le joueur a l'endroit demande
    /// </summary>
    void PlayerPlacer()
    {
        playerKnight.Initialize(playerPosition);
    }

    /// <summary>
    /// Place les ennemis aux endroits demandes
    /// </summary>
    void EnemyPlacer()
    {
        enemies = new List<GameObject>();
        foreach(Vector2 enemyPosition in enemyPositions)
        {
            enemies.Add(Instantiate(enemyPrefab));
            enemies.Last().GetComponent<AEnemy>().Initialize(enemyPosition);
        }
    }
    
    /// <summary>
    /// Le premier loop du jeu, jusqu'a ce que le joueur reprenne la main
    /// </summary>
    void FirstLoop()
    {
        foreach (GameObject enemy in enemies) enemy.GetComponent<AEnemy>().PathFinding();
        //MonstreInterface
        playerKnight.AllowMovement();
    }

    /// <summary>
    /// Declenche la nouvelle scene si le joueur atteint la fin du niveau
    /// </summary>
    void CheckSceneEnd()
    {
        if ((Vector2)playerKnight.transform.position == endPosition) SceneManager.LoadScene(nextLevel);
    }

    /// <summary>
    /// Utilisee pour remettre a zero toutes les tuiles de la grille (niveau couleur)
    /// </summary>
    void GridReset()
    {
        for(int i = 0; i < levelWidth; i++) for(int j = 0; j< levelHeight; j++)
            {
                if (dungeonGrid[i, j] != null && dungeonGrid[i, j].GetComponent<TileScript>().getAltered()) dungeonGrid[i, j].GetComponent<TileScript>().resetColor();
            }
    }

    /// <summary>
    /// Une boucle de jeu complete, qui se termine avec le joueur reprennant la main
    /// </summary>
    public void NewLoop()
    {
        CheckSceneEnd();
        foreach (GameObject enemy in enemies) enemy.GetComponent<AEnemy>().Action();
        GridReset();
        foreach (GameObject enemy in enemies) enemy.GetComponent<AEnemy>().PathFinding();
        //MonstreInterface
        playerKnight.AllowMovement();
    }

    /// <summary>
    /// Indique si le mouvement est possible, invalide a cause du vide, ou invalide a cause d'un ennemi
    /// </summary>
    /// <param name="x">coordonne x de la case de destination</param>
    /// <param name="y">coordonne y de la case de destination</param>
    /// <returns>0 si le mouvement est impossible, 1 si il est possible, 2 si il est possible mais bloquer par un ennemi</returns>
    public int AuthorizeMovement(int x, int y)
    {
        //On check les bordures de base
        if (x < startX || x > startX + levelWidth - 1) return 0;
        if (y < startY || y > startY + levelHeight - 1) return 0;

        //On check les trous
        if (dungeonGrid[x - startX, y - startY] == null) return 0;

        //On check les entites
        if (playerKnight.transform.position.x == x && playerKnight.transform.position.y == y) return 2;
        else foreach (GameObject enemy in enemies) if (enemy.transform.position.x == x && enemy.transform.position.y == y) return 2;

        return 1;
    }

    /// <summary>
    /// Indique si le mouvement est possible ou non (si le mouvement envoie dans le vide ou non)
    /// </summary>
    /// <param name="vec">les coordonnees desirees, donnees sous la forme de vecteur</param>
    /// <returns>0 si le mouvement est impossible, 1 si il est possible. 2 si il est possible mais bloquer par un ennemi</returns>
    public int AuthorizeMovement(Vector3 vec)
    {
        return AuthorizeMovement((int)vec.x, (int)vec.y);
    }

    /// <summary>
    /// Cherche l'ennemi qu'on pourrait viser
    /// </summary>
    /// <param name="vec">La case qu'on veut interroger</param>
    /// <returns>null si il y a personne, ou un gameobject si il y a quelqu'un ici</returns>
    public GameObject GetEnemy(Vector2 vec)
    {
        foreach (GameObject enemy in enemies) if ((Vector2)enemy.transform.position == vec) return enemy;
        return null;
    }

    /// <summary>
    /// Utilisez par les ennemis pour qu'ils se retirent de notre liste
    /// </summary>
    /// <param name="enemy">l'ennemi a retirer</param>
    public void EnemyRemover(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// Utilise par les monstres pour indiquer ou va leur prochain mouvement, et quel type de mouvement c'est
    /// </summary>
    /// <param name="x">Coordonnee en x du mouvement</param>
    /// <param name="y">Coordonnee en y du mouvement</param>
    /// <param name="intentionAttaque">Est-ce que l'action est une attaque ou est-ce que c'est un mouvement</param>
    public void EnemyIntention(int x, int y, bool intentionAttaque)
    {
        if(dungeonGrid[x - startX, y - startY] != null)
        {
            if (!intentionAttaque) dungeonGrid[x - startX, y - startY].GetComponent<TileScript>().changeColor(mobIntentions[0]);
            else dungeonGrid[x - startX, y - startY].GetComponent<TileScript>().changeColor(mobIntentions[1]);
        }
    }

    /// <summary>
    /// Renvoit la position du joueur
    /// </summary>
    /// <returns>Le transform.position du jouer</returns>
    public Vector3 getPlayerPosition() => playerKnight.transform.position;

    /// <summary>
    /// Renvoit le gameObject joueur pour que les monstres puissent l'endommager
    /// </summary>
    /// <returns>le gameobject joueur</returns>
    public GameObject getPlayer() => playerKnight.gameObject;

    /// <summary>
    /// Permet de recharger la scene actuelle pour recommencer
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
