using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMasterScript : MonoBehaviour
{
    //Public
    public int levelWidth, levelHeight; //Les dimensions de notre dongeon en terme de nombre de tuiles
    public Vector2 playerPosition; //La position du joueur au debut du niveau
    public Vector2 endPosition; //La position de la tuile de fin de niveau
    public string nextLevel; //Le nom de la scene suivante
    public PlayerKnightScript playerKnight; //Le gameObject du joueur
    public GameObject tilePrefab; //Pour stocker le prefab de la tuile

    //Private
    
    int startN, startM; //Point de depart des coordonnes (dans le negatif)
    GameObject[,] dungeonGrid; //Le sol du donjon, represente comme une grille
    GameObject currentPrefab; //Le prefab qu'on vient d'instancier
    Color[] colors = new Color[2] { new Color(145f / 255, 205f / 255, 220f / 255), new Color(105f / 255, 165f / 255, 180f / 255) }; //Les deux couleurs utilisees pour la quinquonce de notre damier
    Color endTile = new Color(125f / 255, 185f / 255, 200f / 255);
    int pairImpair; //Juste pour savoir si on est sur une tuile paire ou impaire (initialisation uniquement)


    /// <summary>
    /// Appeler au debut de la scene, pour tout initialiser
    /// </summary>
    void Start()
    {
        Begin();
    }

    /// <summary>
    /// La fonction utilise pour (re)initialiser la scence
    /// </summary>
    void Begin()
    {
        FloorBuilder();
        PlayerPlacer();
    }

    /// <summary>
    /// Creer le sol du donjon grace a la grille de tuiles
    /// </summary>
    void FloorBuilder()
    {
        startN = -levelWidth / 2;
        startM = -levelHeight / 2;

        dungeonGrid = new GameObject[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++) for (int j = 0; j < levelHeight; j++)
            {
                dungeonGrid[i, j] = Instantiate(tilePrefab);
                pairImpair = (i + j) % 2;
                if(startN + i == endPosition.x && startM + j == endPosition.y) dungeonGrid[i, j].GetComponent<TileScript>().Initialize(startN + i, startM + j, endTile);
                else dungeonGrid[i, j].GetComponent<TileScript>().Initialize(startN + i, startM + j, colors[pairImpair]);
            }
    }

    /// <summary>
    /// Place le joueur a l'endroit demande
    /// </summary>
    void PlayerPlacer()
    {
        playerKnight.transform.position = playerPosition;
    }

    /// <summary>
    /// Indique si le mouvement est possible ou non (si le mouvement envoie dans le vide ou non)
    /// </summary>
    /// <param name="x">coordonne x de la case de destination</param>
    /// <param name="y">coordonne y de la case de destination</param>
    /// <returns>true si le mouvement reste sur du sol, false sinon</returns>
    public bool AuthorizeMovement(int x, int y)
    {
        if (x < startN || x > startN + levelWidth - 1) return false;
        if (y < startM || y > startM + levelHeight - 1) return false;
        return true;
    }

    /// <summary>
    /// Indique si le mouvement est possible ou non (si le mouvement envoie dans le vide ou non)
    /// </summary>
    /// <param name="vec">les coordonnees desirees, donnees sous la forme de vecteur</param>
    /// <returns>true si le mouvement reste sur du sol, false sinon</returns>
    public bool AuthorizeMovement(Vector3 vec)
    {
        return AuthorizeMovement((int)vec.x, (int)vec.y);
    }

    public void EndLevelChecker()
    {
        if ((Vector2)playerKnight.transform.position == endPosition) SceneManager.LoadScene(nextLevel);
    }
}
