using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMasterScript : MonoBehaviour
{
    //Public
    public bool Floor13; //Est-ce qu'on est a l'etage infini ?
    public int levelWidth, levelHeight; //Les dimensions de notre dongeon en terme de nombre de tuiles
    public List<Vector2> holesInTheFloor; //La ou il n'y a pas de sol dans notre niveau
    public List<Vector2> enemyPositions; //La ou on va mettre nos ennemis
    public List<Vector2> treasures; //Les 3 tresors dans la salle
    public Vector2 playerPosition; //La position du joueur au debut du niveau
    public Vector2 endPosition; //La position de la tuile de fin de niveau
    public string nextLevel; //Le nom de la scene suivante
    public PlayerKnightScript playerKnight; //Le gameObject du joueur
    public GameObject tilePrefab; //Pour stocker le prefab de la tuile
    public GameObject endPrefab; //Pour stocker le prefab de la tuile de fin
    public GameObject fallPrefab; //Pour stocker le prefab des trous du dongeon
    public GameObject treasurePrefab; //Pour stocker le prefab des tresors dans le donjon
    public GameObject enemyPrefab; //Pour stocker le prefab de l'ennemi
    public GameObject secondEnemyPrefab; //L'autre prefab ennemi, pour le niveau 13
    public GameObject coinPrefab; //Les pieces lorsqu'on choppe un tresor

    //Private
    int startX, startY; //Point de depart des coordonnes (dans le negatif)
    GameObject[,] dungeonGrid; //Le sol du donjon, represente comme une grille
    GameObject[,] dungeonFalls; //Pour les trous dans le niveau
    List<GameObject> dungeonTreasures; //Pour les tresors du niveau
    Color[] colors = new Color[2] { new Color(173f / 255, 199f / 255, 204f / 255), new Color(145f / 255, 170f / 255, 194f / 255) }; //Les deux couleurs utilisees pour la quinquonce de notre damier
    Color endTile = new Color(161f / 255, 188f / 255, 201f / 255); //La couleur utilise par la tuile de fin de niveau
    Color[] mobIntentions = new Color[2] { new Color(108f / 255, 217f / 255, 126f / 255), new Color(217f / 255, 108f / 255, 126f / 255) }; //Les couleurs utilisees par "l'interface", respectivement deplacement puis attaque
    int pairImpair; //Juste pour savoir si on est sur une tuile paire ou impaire (initialisation uniquement)
    List<GameObject> enemies; // La liste de tous les ennemis actuellement presents dans le niveau
    List<Vector2> casesPassees, casesEnCours, casesFutures, voisinsVises; //Utilisees pour l'algorithme d'attribution de distance, pour stocker les traitements
    int manhattanDistance; //Aussi utilise pour l'attribution de distance
    int distanceMin; //Utilisee pour donner le meilleur lors du pathfinding des monstres
    int currentSkeleton; //A quel ennemi en est-on ?
    AudioManagerScript audioManager; //Utilise pour tous les sons
    int nbCoins; //Utilise par les particules pour savoir combien il y en a
    GameObject currentCoin; //La piece qu'on vient d'initialiser

    /// <summary>
    /// Appeler au debut de la scene, pour tout initialiser
    /// </summary>
    void Start()
    {
        if (Floor13) RessourcesAppropriator();
        AspectRatioCalculator();
        PathFinderInitializer();
        Begin();
        FirstLoop();
    }

    /// <summary>
    /// Vole les variables statiques de la classe faite pour ca
    /// </summary>
    void RessourcesAppropriator()
    {
        //On commence par demander une generation du niveau, si on a gagne precedemment
        if(GameMasterRessources.weWon) GameMasterRessources.Floor13Generator();
        //On vole mainteant les ressources du nouveau niveau
        levelWidth = GameMasterRessources.levelWidth;
        levelHeight = GameMasterRessources.levelHeight;
        playerPosition = GameMasterRessources.playerPosition;
        endPosition = GameMasterRessources.endPosition;
        holesInTheFloor = GameMasterRessources.holesInTheFloor;
        enemyPositions = GameMasterRessources.enemyPositions;
        treasures = GameMasterRessources.treasures;
    }

    /// <summary>
    /// Calcule la composante hauteur de la camera pour accueillir le niveau de maniere optimale
    /// </summary>
    void AspectRatioCalculator()
    {
        Camera.main.orthographicSize = (3.6f / Screen.width) * Screen.height;
    }

    /// <summary>
    /// Juste pour initialiser nos differentes listes pour les distances de manhattan (ansi que l'audio manager)
    /// </summary>
    void PathFinderInitializer()
    {
        casesPassees = new List<Vector2>();
        casesEnCours = new List<Vector2>();
        casesFutures = new List<Vector2>();
        voisinsVises = new List<Vector2>();
        //Il fallait bien le mettre quelque part
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    /// <summary>
    /// La fonction utilise pour (re)initialiser la scence
    /// </summary>
    void Begin()
    {
        FloorBuilder();
        NeighborCreator();
        PlayerPlacer();
        EnemyPlacer();
    }

    /// <summary>
    /// Creer le sol du donjon grace a la grille de tuiles
    /// </summary>
    void FloorBuilder()
    {
        //Initialisation de variable
        startX = -levelWidth / 2;
        startY = -levelHeight / 2;

        //On commence par poser les tuiles
        int tempX, tempY;
        dungeonGrid = new GameObject[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++) for (int j = 0; j < levelHeight; j++)
            {
                tempX = startX + i;
                tempY = startY + j;
                //Si la tuile n'est pas un trou
                if(!holesInTheFloor.Contains(new Vector2(tempX, tempY)))
                {
                    //Pas le meme modele selon si la tuile est l'escalier ou une tuile classique
                    if (tempX == endPosition.x && tempY == endPosition.y) dungeonGrid[i, j] = Instantiate(endPrefab);
                    else dungeonGrid[i, j] = Instantiate(tilePrefab);
                    //On s'apprete a initialiser, et on calcul la couleur pour faire du quinquonce
                    pairImpair = (i + j) % 2;
                    if (tempX == endPosition.x && tempY == endPosition.y) dungeonGrid[i, j].GetComponent<TileScript>().Initialize(tempX, tempY, endTile);
                    else dungeonGrid[i, j].GetComponent<TileScript>().Initialize(tempX, tempY, colors[pairImpair]);
                }
            }

        //On peut passer aux trous
        dungeonFalls = new GameObject[levelWidth, levelHeight + 1];
        for (int i = 0; i < levelWidth; i++) for (int j = 0; j < levelHeight - 1; j++)
            {
                //On cherche juste les endroits ou la tuile est null mais pas la tuile du dessus
                if (dungeonGrid[i, j] == null && dungeonGrid[i, j + 1] != null)
                {
                    tempX = startX + i;
                    tempY = startY + j;
                    dungeonFalls[i, j + 1] = Instantiate(fallPrefab);
                    pairImpair = (i + j + 1) % 2;
                    dungeonFalls[i, j + 1].GetComponent<TileScript>().Initialize(tempX, tempY, colors[pairImpair]);
                }
            }
        //On finit par les bordures en bas du niveau
        for (int i = 0; i < levelWidth; i++) if (dungeonGrid[i, 0] != null)
            {
                tempX = startX + i;
                tempY = startY - 1;
                dungeonFalls[i, 0] = Instantiate(fallPrefab);
                pairImpair = (i) % 2;
                dungeonFalls[i, 0].GetComponent<TileScript>().Initialize(tempX, tempY, colors[pairImpair]);
            }

            //On finit par placer les tresors dans le niveau
            dungeonTreasures = new List<GameObject>();
        foreach(Vector2 treasure in treasures)
        {
            dungeonTreasures.Add(Instantiate(treasurePrefab));
            dungeonTreasures.Last().GetComponent<TileScript>().Initialize((int)treasure.x, (int)treasure.y, Color.white);
            dungeonTreasures.Last().GetComponent<TileScript>().ChaneSortingOrder();
        }
    }

    /// <summary>
    /// Attribut a chaque case les coordonnees de ses voisins
    /// </summary>
    void NeighborCreator()
    {
        for(int i = 0; i < levelWidth; i++) for(int j = 0; j < levelHeight; j++)
            {
                if (dungeonGrid[i, j] != null)
                {
                    if ((i - 1) >= 0 && dungeonGrid[i - 1, j] != null)
                        dungeonGrid[i, j].GetComponent<TileScript>().AddNeighbor(new Vector2(i - 1, j));
                    if ((i + 1) < levelWidth && dungeonGrid[i + 1, j] != null)
                        dungeonGrid[i, j].GetComponent<TileScript>().AddNeighbor(new Vector2(i + 1, j));
                    if ((j - 1) >= 0 && dungeonGrid[i, j - 1] != null)
                        dungeonGrid[i, j].GetComponent<TileScript>().AddNeighbor(new Vector2(i, j - 1));
                    if ((j + 1) < levelHeight && dungeonGrid[i, j + 1] != null)
                        dungeonGrid[i, j].GetComponent<TileScript>().AddNeighbor(new Vector2(i, j + 1));
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
        for(int i = 0; i < enemyPositions.Count; i++)
        {
            //Si on est pas au niveau 13, on a qu'un seul type d'ennemi
            if (!Floor13)
            {
                enemies.Add(Instantiate(enemyPrefab));
            }
            //Si on est au niveau 13, un ennemi sur trois est a distance
            else
            {
                if (i % 3 == 0) enemies.Add(Instantiate(secondEnemyPrefab));
                else enemies.Add(Instantiate(enemyPrefab));
            }
            enemies.Last().GetComponent<AEnemy>().Initialize(enemyPositions[i]);
        }
    }
    
    /// <summary>
    /// Le premier loop du jeu, jusqu'a ce que le joueur reprenne la main
    /// </summary>
    void FirstLoop()
    {
        DistanceAssignment();
        foreach (GameObject enemy in enemies) enemy.GetComponent<AEnemy>().PathFinding();
        playerKnight.AllowMovement();
    }

    /// <summary>
    /// Donne a chaque case un entier, qui represente sa distance avec le joueur (distance Manhattan)
    /// </summary>
    void DistanceAssignment()
    {
        casesEnCours.Add(new Vector2(playerKnight.transform.position.x - startX, playerKnight.transform.position.y - startY));
        manhattanDistance = 0;
        casesPassees.Clear();

        //Tant qu'on trouve des cases futures, on a pas fait le tour donc on continue
        do
        {
            //On inspecte nos cases a inspecter
            foreach(Vector2 coordonnees in casesEnCours)
            {
                //On commence par leur donner leur distance
                dungeonGrid[(int)coordonnees.x, (int)coordonnees.y].GetComponent<TileScript>().setDistance(manhattanDistance);
                //On recupere les voisins de la case en cours
                voisinsVises = dungeonGrid[(int)coordonnees.x, (int)coordonnees.y].GetComponent<TileScript>().getNeighbors();
                //On les inspecte a leur tour, ne gardant que les voisins non null qu'on a pas deja visite
                foreach (Vector2 voisin in voisinsVises) if (!casesPassees.Contains(voisin) && !casesFutures.Contains(voisin) && dungeonGrid[(int)voisin.x, (int)voisin.y] != null) casesFutures.Add(voisin);
            }
            //On incremente enfin notre distance apres notre boucle
            manhattanDistance++;

            //On rajoute les cases qu'on vient de traiter dans la liste du deja fait, et on s'approprie les cases futures
            foreach (Vector2 caseencours in casesEnCours) casesPassees.Add(caseencours);
            casesEnCours.Clear();
            foreach (Vector2 casefuture in casesFutures) casesEnCours.Add(casefuture);
            casesFutures.Clear();

        //On s'arrete lorsqu'il n'y a plus de cases a traiter
        } while (casesEnCours.Count != 0);
    }

    /// <summary>
    /// Une boucle de jeu complete, qui se termine avec le joueur reprennant la main
    /// </summary>
    public void NewLoop()
    {
        CheckSceneEnd();
        CheckTreasure();
        GridReset();
        currentSkeleton = -1;
        NextSkeleton();
    }

    /// <summary>
    /// Declenche la nouvelle scene si le joueur atteint la fin du niveau
    /// </summary>
    void CheckSceneEnd()
    {
        if ((Vector2)playerKnight.transform.position == endPosition)
        {
            if (Floor13) audioManager.Play("Triumph");
            GameMasterRessources.weWon = true;
            SceneManager.LoadScene(nextLevel);
        }
    }

    /// <summary>
    /// On regarde si le joueur ramasse un tresor ou non apres son deplacement
    /// </summary>
    void CheckTreasure()
    {
        //On inspecte nos tresors
        for(int i = dungeonTreasures.Count - 1; i >= 0; i--)
        {
            //Si on a les memes coordonnees qu'un tresor
            if(dungeonTreasures[i].transform.position == playerKnight.transform.position)
            {
                nbCoins = Random.Range(1, 6);
                for (int j = 0; j < nbCoins; j++)
                {
                    currentCoin = GameObject.Instantiate(coinPrefab);
                    currentCoin.GetComponent<ParticleScript>().Initialize(playerKnight.transform.position);
                }

                audioManager.Play("TreasurePickedUp");
                playerKnight.ScoreTreasure();
                dungeonTreasures[i].SetActive(false);
                dungeonTreasures.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Utilisee pour remettre a zero toutes les tuiles de la grille (niveau couleur)
    /// </summary>
    void GridReset()
    {
        for (int i = 0; i < levelWidth; i++) for (int j = 0; j < levelHeight; j++)
            {
                if (dungeonGrid[i, j] != null && dungeonGrid[i, j].GetComponent<TileScript>().getAltered()) dungeonGrid[i, j].GetComponent<TileScript>().ResetColor();
            }
    }

    /// <summary>
    /// Ordonne au squelette suivant d'agir
    /// </summary>
    public void NextSkeleton()
    {
        currentSkeleton++;
        if (currentSkeleton >= enemies.Count) FirstLoop();
        else
        {
            if (enemies[currentSkeleton].GetComponent<DummyScript>() != null) enemies[currentSkeleton].GetComponent<DummyScript>().Action();
            else enemies[currentSkeleton].GetComponent<AEnemy>().Action();
        }
    }

    /// <summary>
    /// Indique si le mouvement est possible, invalide a cause du vide, ou invalide a cause d'un ennemi
    /// </summary>
    /// <param name="x">coordonne x de la case de destination</param>
    /// <param name="y">coordonne y de la case de destination</param>
    /// <returns>-1 si le mouvement est en dehors du niveau
    ///          0 si le mouvement est au dessus d'un trou
    ///          1 si le mouvement est libre
    ///          2 si le mouvement est sur une entite</returns>
    public int AuthorizeMovement(int x, int y)
    {
        //On check les bordures de base
        if (x < startX || x > startX + levelWidth - 1) return -1;
        if (y < startY || y > startY + levelHeight - 1) return -1;

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
    public GameObject GetEntity(Vector2 vec)
    {
        if ((Vector2)playerKnight.transform.position == vec) return playerKnight.gameObject;
        else foreach (GameObject enemy in enemies) if ((Vector2)enemy.transform.position == vec) return enemy;
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
            if (!intentionAttaque) dungeonGrid[x - startX, y - startY].GetComponent<TileScript>().ChangeColor(mobIntentions[0]);
            else dungeonGrid[x - startX, y - startY].GetComponent<TileScript>().ChangeColor(mobIntentions[1]);
        }
    }

    /// <summary>
    /// Renvoit la position du joueur
    /// </summary>
    /// <returns>Le transform.position du jouer</returns>
    public Vector3 getPlayerPosition() => playerKnight.transform.position;

    /// <summary>
    /// Utilise par les monstres pour savoir quel mouvement les rapprochera le plus du joueur
    /// </summary>
    /// <param name="coordonnees">Les coordonnes actuelles du monstre</param>
    /// <returns>Les coordonnees a viser</returns>
    public Vector2 PathFinding(Vector2 coordonnees)
    {
        coordonnees.x -= startX;
        coordonnees.y -= startY;

        //On s'initialise en trouvant les voisins, la distance a battre est la distance qui nous separe du joueur actuellement
        voisinsVises = dungeonGrid[(int)coordonnees.x, (int)coordonnees.y].GetComponent<TileScript>().getNeighbors();
        distanceMin = dungeonGrid[(int)coordonnees.x, (int)coordonnees.y].GetComponent<TileScript>().getDistance();
        foreach(Vector2 voisin in voisinsVises)
        {
            //Si la distance est clairement meilleure on prend
            if (dungeonGrid[(int)voisin.x, (int)voisin.y].GetComponent<TileScript>().getDistance() < distanceMin)
            {
                distanceMin = dungeonGrid[(int)voisin.x, (int)voisin.y].GetComponent<TileScript>().getDistance();
                coordonnees = voisin;
            }
            //Si la distance est equivalente, une chance sur deux de prendre
            else if (dungeonGrid[(int)voisin.x, (int)voisin.y].GetComponent<TileScript>().getDistance() == distanceMin && Random.value < 0.5f)
            {
                distanceMin = dungeonGrid[(int)voisin.x, (int)voisin.y].GetComponent<TileScript>().getDistance();
                coordonnees = voisin;
            }
        }

        coordonnees.x += startX;
        coordonnees.y += startY;

        return coordonnees;
    }

    /// <summary>
    /// Permet de recharger la scene actuelle pour recommencer
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
