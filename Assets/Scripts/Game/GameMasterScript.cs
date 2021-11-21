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
    Color[] colors = new Color[2] { new Color(145f / 255, 205f / 255, 220f / 255), new Color(105f / 255, 165f / 255, 180f / 255) }; //Les deux couleurs utilisees pour la quinquonce de notre damier
    Color endTile = new Color(125f / 255, 185f / 255, 200f / 255); //La couleur utilise par la tuile de fin de niveau
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
        Camera.main.orthographicSize = ((levelWidth / 2f + 0.2f) / Screen.width) * Screen.height;
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
            enemies.Last().GetComponent<DummyScript>().Initialize(enemyPosition);
        }
    }
    
    /// <summary>
    /// Le premier loop du jeu, jusqu'a ce que le joueur reprenne la main
    /// </summary>
    void FirstLoop()
    {
        //MonstrePathFinding
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
        //MonstreActions
        GridReset();
        //MonstrePathFinding
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
}
