using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    /// <summary>
    /// Pour charger la toute premiere scene et faire les tutoriels
    /// </summary>
    public void LaunchTutorial()
    {
        SceneManager.LoadScene("Level01");
    }

    /// <summary>
    /// Pour lancer l'etage 13
    /// </summary>
    public void LaunchFloor13()
    {
        SceneManager.LoadScene("Level13");
    }
}
