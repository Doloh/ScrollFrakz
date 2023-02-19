using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float playerSpeed;
    public float jumpForce;

    private bool isGround = true;
    private bool canMove = true;
    private bool faceRight = true;
    private Rigidbody2D body;

    //START
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    //FIXED UPDATE
    private void FixedUpdate()
    {
        Deplacements();
        Jump();
    }

    //Tant que l'on reste en collision avec un autre objet.
    void OnCollisionStay2D(Collision2D _collision)
    {
        if(_collision.gameObject.tag == "sol")
        {
            isGround = true;
        } 
    }
   

    void Deplacements()
    {       
        //recupere l'entrée horizontale (float allant de -1 à 1), et adapte la direction du sprite en fonction
        float deplacementsHorizontaux = Input.GetAxis("Horizontal");
        FlipSprite(deplacementsHorizontaux);

        //definir la velocite du personnage
        //Je determine la direction du player grace au bool faceRight
        float velocite = deplacementsHorizontaux * playerSpeed * Time.deltaTime;
        Vector2 playerDirection = faceRight ? Vector2.right : Vector2.left;
       

        // Obtenir les dimensions du sprite
        // Déterminer la position de départ du raycast
        // lancer un raycast dans le direction du personnage
        // Dessiner le Raycast dans la scène
        Vector2 spriteDimensions = GetComponent<Collider2D>().bounds.size;
        Vector2 raycastStart = new Vector2(transform.position.x + (faceRight ? spriteDimensions.x / 2f + 0.01f : -spriteDimensions.x / 2f - 0.01f), transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, playerDirection, 0.1f);
        Debug.DrawRay(raycastStart, playerDirection * 0.1f, Color.red);

        //DEPLACEMENTS
        if (hit.collider == null || !hit.collider.gameObject.CompareTag("obstacle")) //Si il n'y a pas d'obstacle
        {
            if (canMove && deplacementsHorizontaux != 0) 
            {
                body.velocity = new Vector2(velocite, body.velocity.y);
                //body.AddForce(new Vector2(velocite, body.velocity.y), ForceMode2D.Force);
            }
        } 
        else // Si il y a un obstacle
        {
            body.velocity = new Vector2(0f, body.velocity.y);
            // JBONK AERIEN
            float JbonkForce = 2f;
            JbonkAerien(JbonkForce);
        }    
 
    }

    void Jump()
    {
        if (Input.GetAxisRaw("Saut") > 0 && isGround)
        {
            // Applique une force de saut sur le corps rigide en fonction de la durée du saut
            body.AddForce(Vector2.up * jumpForce * Time.deltaTime);
            isGround = false;                    
        }
    }

    //FONCTION qui appelle un flip du sprite en cas de changement de direction du joueur.
    void FlipSprite(float _deplacementsHorizontaux)
    {
        if (_deplacementsHorizontaux > 0 && !faceRight)
        {
            Flip();
        }
        else if (_deplacementsHorizontaux < 0 && faceRight)
        {
            Flip();
        }        
    }

    //FONCTION permettant de switch la direction droite/gauche du sprite 
    void Flip()
    {
         transform.Rotate(0f, 180f, 0f);
         faceRight = !faceRight;
    }

    //FONCTION permettant de faire Jbonk le perso si il est dans les airs, en fonction de la direction dans laquelle il regarde
    void JbonkAerien(float _JbonkForce)
    {
        if (!isGround)
        {            
            StartCoroutine(JbonkAerienCoroutine(_JbonkForce));            
        }
    }

    //Coroutine permettant de faire Jbonk le perso si il est dans les airs, en fonction de la direction dans laquelle il regarde
    IEnumerator JbonkAerienCoroutine(float _jbonkForce)
    {
        canMove = false;
        if (faceRight)
        {
            body.AddForce(Vector2.left * _jbonkForce, ForceMode2D.Impulse);
            yield return new WaitUntil(() => isGround);
            canMove = true;
        } 
        else
        {
            body.AddForce(Vector2.right * _jbonkForce, ForceMode2D.Impulse);
            yield return new WaitUntil(() => isGround);
            canMove = true;
        }
    }

}
