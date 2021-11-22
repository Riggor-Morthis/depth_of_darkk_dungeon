using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterRessources : MonoBehaviour
{
    //Public static
    public static int levelWidth, levelHeight;
    public static Vector2 playerPosition, endPosition;
    public static List<Vector2> holesInTheFloor;
    public static List<Vector2> enemyPositions;
    public static List<Vector2> treasures;
    public static bool weWon = true; //Indique que le niveau precedent a ete gagne, et qu'il faut donc changer la generation

    //Private
    static int startX, endX;
    static int startY, endY;
    static int budget; //utilise a differents moments pour savoir combien d'objets peuvent etre places
    static int currentNumber; //utilise pour stocker differents decomptes
    static Vector2 currentVec; //utilise pour stocker les vecteurs aleatoire
    static Vector2 lastHole; //utilise pour faire des trous
    static int xHole, yHole; //juste pour placer des trous la
    static int dealBreaker; //si on prend trop de temps pour trouver ou placer un trou, on arrete
    static float currentDistance; //la distance actuel entre une entite et une autre

    /// <summary>
    /// Utilise pour generer les differents niveaux 13, le niveau infini
    /// </summary>
    public static void Floor13Generator()
    {
        //Avant toute chose, on a pas gagne encore
        weWon = false;

        //Premierement, les dimensions du niveau
        levelWidth = 7 + Random.Range(0, 5) * 2;
        levelHeight = 22 - levelWidth;
        startX = -(levelWidth / 2);
        endX = -startX;
        startY = -(levelHeight / 2);
        endY = -startY;
        //Ensuite, on place l'entree (le joueur) et la sortie
        playerPosition = new Vector2(Random.Range(startX, endX), startY);
        endPosition = -playerPosition;

        //Faisons des trous dans le niveau
        //On trouve combien de trous il faut faire
        budget = Mathf.RoundToInt(Random.Range(0.05f, 0.20f) * (levelHeight * levelWidth));
        holesInTheFloor = new List<Vector2>();
        do
        {
            //On fait des coordonnees au pif
            currentVec = new Vector2(Random.Range(startX, endX + 1), Random.Range(startY, endY + 1));
            lastHole = currentVec;
            //On va lui rajouter un nombre aleatoire de copain
            currentNumber = Mathf.Max(Random.Range(1, 10), budget);
            for(int i = 0; i < currentNumber; i++)
            {
                dealBreaker = 0;
                do
                {
                    //On essaye de trouver de nouvelles coordonnees dans la grille
                    xHole = (int)lastHole.x + Random.Range(-1, 2);
                    if (xHole < startX) xHole = startX;
                    else if (xHole > endX) xHole = endX;

                    yHole = (int)lastHole.y + Random.Range(-1, 2);
                    if (yHole < startY) yHole = startY;
                    else if (yHole > endY) yHole = endY;

                    currentVec = new Vector2(xHole, yHole);
                    //On a une condition d'arret express si on prend trop d'essais a trouver le trou
                    dealBreaker++;
                } while ((currentVec == playerPosition || currentVec == endPosition || holesInTheFloor.Contains(currentVec)) && dealBreaker < 8);
                //On a atteint la condition d'arret express, on placera un trou de moins pour generer de nouvelles coordonnees
                if(dealBreaker == 8) currentVec = new Vector2(Random.Range(startX, endX + 1), Random.Range(startY, endY + 1));
                //On a echappe a la condition d'arret express, on peut rajouter le trou dans la liste
                else
                {
                    holesInTheFloor.Add(currentVec);
                    lastHole = currentVec;
                }
            }

            budget -= currentNumber;
        } while (budget > 0);

        //On va s'assurer qu'on peut atteindre la fin, peu importe les trous
        //On va faire ca de maniere tres simpliste et brutale
        for (int i = startY; i <= endY; i++)
        {
            currentVec = new Vector2(playerPosition.x, i);
            if (holesInTheFloor.Contains(currentVec)) holesInTheFloor.Remove(currentVec);
            currentVec = new Vector2(endPosition.x, i);
            if (holesInTheFloor.Contains(currentVec)) holesInTheFloor.Remove(currentVec);
        }
        for (int i = -Mathf.Abs((int)playerPosition.x); i < Mathf.Abs((int)playerPosition.x); i++)
        {
            currentVec = new Vector2(i, startY);
            if (holesInTheFloor.Contains(currentVec)) holesInTheFloor.Remove(currentVec);
            currentVec = new Vector2(i, endY);
            if (holesInTheFloor.Contains(currentVec)) holesInTheFloor.Remove(currentVec);
        }

        //On va maintenant placer les ennemis
        //On determine combien on en veut
        budget = Mathf.RoundToInt(Random.Range(0.01f, 0.05f) * (levelHeight * levelWidth)) + 1;
        enemyPositions = new List<Vector2>();
        for(int i = 0; i < budget; i++)
        {
            //On invente des coordonnes jusqu'a decouvrir un endroit libre
            do
            {
                currentVec = new Vector2(Random.Range(startX, endX + 1), Random.Range(startY, endY + 1));
                currentDistance = Vector2.Distance(playerPosition, currentVec);
                foreach (Vector2 enemy in enemyPositions) if (currentDistance > Vector2.Distance(enemy, currentVec)) currentDistance = Vector2.Distance(enemy, currentVec);
            } while (currentVec == playerPosition || currentVec == endPosition || holesInTheFloor.Contains(currentVec) || currentDistance <= 3);
            enemyPositions.Add(currentVec);
        }

        //Pour finir, on depose des tresors dans la salle
        budget = Mathf.RoundToInt(Random.Range(0.03f, 0.05f) * (levelHeight * levelWidth));
        treasures = new List<Vector2>();
        for (int i = 0; i < budget; i++)
        {
            do
            {
                currentVec = new Vector2(Random.Range(startX, endX + 1), Random.Range(startY, endY + 1));
            } while (currentVec == playerPosition || currentVec == endPosition || holesInTheFloor.Contains(currentVec) || enemyPositions.Contains(currentVec) || treasures.Contains(currentVec));
            treasures.Add(currentVec);
        }
    }
}
