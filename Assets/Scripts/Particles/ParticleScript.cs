using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    //private
    float moveX; //Le mouvement en x de notre casque
    float moveY; //Le mouvement en y de notre casque
    float downSpeed; //La vitesse vers le bas de notre casque
    Vector3 currentPosition; //Notre position actuelle
    float rotationSpeed; //La vitesse a laquelle le casque tourne
    float currentAlpha; //Notre alpha actuel
    float dyingSpeed; //La vitesse a laquelle notre alpha disparait
    SpriteRenderer spriteRenderer; //Notre render pour notre alpha

    // Update is called once per frame
    void Update()
    {
        if (currentAlpha <= 0) gameObject.SetActive(false);
        else
        {
            currentAlpha -= dyingSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentAlpha / 255);
        }

        downSpeed -= 10 * Time.deltaTime;

        currentPosition = transform.position;
        currentPosition.x += moveX * Time.deltaTime;
        currentPosition.y += (moveY + downSpeed) * Time.deltaTime;
        transform.position = currentPosition;

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public void Initialize(Vector3 position)
    {
        moveX = Random.Range(-1f, 1f);
        moveY = Random.Range(0.5f, 1.5f);
        downSpeed = 0;

        currentPosition = position;
        currentPosition.x += moveX * Time.deltaTime;
        currentPosition.y += (moveY + downSpeed) * Time.deltaTime;
        transform.position = currentPosition;

        rotationSpeed = Random.Range(90f, 360f);
        if (Random.value < 0.5f) rotationSpeed = -rotationSpeed;

        currentAlpha = 255;
        dyingSpeed = 255;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 50;
    }
}
