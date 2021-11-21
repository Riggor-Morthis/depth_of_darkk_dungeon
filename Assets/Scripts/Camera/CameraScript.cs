using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Private
    Vector3 target; //La ou on voudrait emmener la camera
    float Speed; //La vitesse a laquelle on ammene la camera la ou on veut l'amener

    /// <summary>
    /// Petit a petit, on ramene la camera a sa place
    /// </summary>
    private void Update()
    {
        if(transform.position != target)
        {
            if (Vector3.Distance(transform.position, target) <= 0.05f) transform.position = target;
            else transform.position += Vector3.up * Speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Utilise pour placer directement la camera verticalement
    /// </summary>
    /// <param name="y">La hauter a laquelle on veut mettre la camera</param>
    public void SetCamera(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        target = transform.position;
    }

    /// <summary>
    /// Utilise pour causer un deplacement doux de la camera
    /// </summary>
    /// <param name="y">la hauteur a laquelle on veut amener la camera</param>
    public void PushCamera(float y)
    {
        target = new Vector3(transform.position.x, y, transform.position.z);
        Speed = (target - transform.position).y * 5;
    }
}
