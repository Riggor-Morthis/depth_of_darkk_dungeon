using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Private
    PlayerKnightScript playerKnight;

    /// <summary>
    /// On récupère le gameobjet du joueur
    /// </summary>
    private void Start()
    {
        playerKnight = GameObject.Find("PlayerKnight").GetComponent<PlayerKnightScript>();
    }

    /// <summary>
    /// On veut juste coller au joueur en permanence
    /// </summary>
    private void Update()
    {
        transform.position = new Vector3(playerKnight.transform.position.x, playerKnight.transform.position.y, transform.position.z);
    }
}
