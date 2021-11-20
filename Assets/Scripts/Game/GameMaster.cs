using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //Public
    public GameObject tilePrefab; //Pour stocker le prefab de la tuile

    //Private
    int n, m; //Les dimensions de notre dongeon en terme de nombre de tuiles
    int startN, startM; //Point de depart des coordonnes (dans le negatif)
    GameObject[,] dungeonGrid; //Le sol du donjon, represente comme une grille
    GameObject currentPrefab; //Le prefab qu'on vient d'instancier
    Color[] colors = new Color[2] { new Color(145f / 255, 205f / 255, 220f / 255), new Color(105f / 255, 165f / 255, 185f / 255) };
    int pairImpair; //Juste pour savoir si on est sur une tuile paire ou impaire (initialisation uniquement)


    // Start is called before the first frame update
    void Start()
    {
        n = 5; m = 5;
        startN = -n / 2;
        startM = -m / 2;

        dungeonGrid = new GameObject[n,m];
        for(int i = 0; i < n; i++) for(int j = 0; j < m; j++)
            {
                dungeonGrid[i,j] = Instantiate(tilePrefab);
                pairImpair = (i + j) % 2;
                dungeonGrid[i, j].GetComponent<TileScript>().Initialize(startN + i, startM + j, colors[pairImpair]);
            }
        
    }
}
