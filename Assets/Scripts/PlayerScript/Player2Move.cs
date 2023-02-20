using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player2Move : MonoBehaviour
{

    public float moveSpeed = 1;
    public float accelerationRate = 9;
    public float deccelerationRate = 9;
    public float velocityPower = 1.2f;
    public float frictionAmount = 10f;

    public bool grounded = true;

    public Rigidbody2D body;
    private Vector2 velocity = Vector2.zero;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Get input
        // Run
        Run();
        Friction();

    }

    void Run()
    {
        // Get l'input horizontal [-1,1]
        float inputX = playerInput.inputX;

        // Calcul la v�locit� cible qu'on veut atteindre via l'input
        float targetVelocity = inputX * moveSpeed;

        
        // Calcul la diff entre la v�locit� actuelle et celle cible
        // au plus l'�cart entre la v�locit� actuelle et la v�locit� � appliquer est grand, au plus on appliquera une grande force 
        // (ex: on se retourne plus vite que si on acc�l�re dans la direction dans laquelle on va d�j�)
        float velocity = targetVelocity - body.velocity.x;

        // Selon si on acc�l�re ou renti, on peut r�gler un facteur d'acc�l�ration diff�rent. 
        // Ces facteurs repr�sentent en gros la vitesse � laquelle on acc�l�re/descelere, en sachant que dans tous les cas avec AddForce �a peut pas �tre instant
        // Et si on mets des values trop grandes (genre 50 - 100) il se passe des trucs chelous ...
        // On applique ensuite ce facteur � notre v�locit�
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? accelerationRate : deccelerationRate; // Si la target speed n'est pas nulle : acc�l�ration, sinon c'est qu'on ralenti
        velocity = velocity * accelRate;

        // On applique une puissance sur la v�locit� pour smooth encore plus le truc 
        // L� j'avoue c'est de la magie noire, j'arrive pas � ressentir ce qu'il y a de diff�rent entre genre : augmenter ce power, ou augmenter l'acc�l�rationRate :/
        float direction = Mathf.Sign(velocity); // On garde la direction en m�moire
        velocity = Mathf.Abs(velocity); // Valeur absolue
        velocity = Mathf.Pow(velocity, velocityPower); // Applique une puissance
        velocity = velocity * direction; // R�applique la direction

        // Debug.Log(velocity);

        body.AddForce(velocity * Vector2.right * Time.deltaTime, ForceMode2D.Impulse);


    }
    
    void Friction()
    {
        // Artificial Friction
        // Si on est au sol et qu'on essaie de s'arr�ter (pas d'input), on va appliquer une anti-force de friction ou notre v�locit� selon lequel des deux est le plus petit.
        if (grounded && Mathf.Abs(playerInput.inputX) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(body.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(body.velocity.x);
            if(amount != 0)
            {
                //Debug.Log(amount);
            }
            body.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
}
