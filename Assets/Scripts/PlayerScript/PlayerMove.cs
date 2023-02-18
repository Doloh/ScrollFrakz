using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float jumpForce = 10f;

    private float deplacementsHorizontaux;
    private bool isGround = true;
    private Rigidbody2D body;
    private bool faceRight = true;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Deplacements();
        Jump();
    }

    void Update()
    {
    }

   
    void Deplacements()
    {       
        //recupere l'entrée horizontale (float allant de -1 à 1)
        deplacementsHorizontaux = Input.GetAxis("Horizontal");
        //definir la velocite du personnage
        float velocite = deplacementsHorizontaux * playerSpeed;


        //Changer la direction du sprite selon la direction droite/gauche
        if (deplacementsHorizontaux > 0 && !faceRight)
        {
            Flip();
        }
        else if (deplacementsHorizontaux < 0 && faceRight)
        {
            Flip();
        }
       
        //Je determine la direction du player grace au bool faceRight
        Vector2 playerDirection = faceRight ? Vector2.right : Vector2.left;

        // Obtenir les dimensions du sprite
        Vector2 spriteDimensions = GetComponent<Collider2D>().bounds.size;
        // Déterminer la position de départ du raycast
        Vector2 raycastStart = new Vector2(transform.position.x + (faceRight ? spriteDimensions.x / 2f + 0.01f : -spriteDimensions.x / 2f - 0.01f), transform.position.y);
       
        //Je lance un raycast dans cette direction
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, playerDirection, 0.1f);
        // Dessiner le Raycast dans la scène
        Debug.DrawRay(raycastStart, playerDirection * 0.1f, Color.red);

        //Si le raycast n'entre pas en collision avec un obstacle, je peux avancer
        if (hit.collider == null || !hit.collider.gameObject.CompareTag("obstacle"))
        {
            //Enfin j'applique la force de deplacement
            body.velocity = new Vector2(velocite, body.velocity.y);
        } else
        {
            body.velocity = Vector2.zero;
        }  
 
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            // Applique une force de saut sur le corps rigide en fonction de la durée du saut
            body.AddForce(Vector2.up * jumpForce);
            isGround = false;                    
        }

    }

    void Flip()
    {
         faceRight = !faceRight;
         transform.Rotate(0f, 180f, 0f);
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "sol")
        {
            isGround = true;
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "obstacle" && !isGround)
        {
            body.velocity = new Vector2(0f, body.velocity.y);
        }
    }

    //SYMPA PERMET DE FAIRE DES JBONK ET D'ATTENDRE QUE LA VELOCITEE DU PERSO RETOMBE A 0 AVANT DE FAIRE AUTRE CHOSE
    //utiliser avec un StartCoroutin(maFonction);

    //IEnumerator JbonkLeft(float jbonkForce)
    //{
    //    Debug.Log("JBONK");
    //    body.AddForce(Vector2.left * jbonkForce, ForceMode2D.Impulse);
    //    yield return new WaitUntil(() => body.velocity == Vector2.zero);
    //    canMove = true;
    //}

    //IEnumerator JbonkRight(float jbonkForce)
    //{
    //    body.AddForce(Vector2.right * jbonkForce, ForceMode2D.Impulse);
    //    yield return new WaitUntil(() => body.velocity == Vector2.zero);
    //    canMove = true;
    //}
}
