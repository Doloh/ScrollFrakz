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

        // Calcul la vélocité cible qu'on veut atteindre via l'input
        float targetVelocity = inputX * moveSpeed;

        
        // Calcul la diff entre la vélocité actuelle et celle cible
        // au plus l'écart entre la vélocité actuelle et la vélocité à appliquer est grand, au plus on appliquera une grande force 
        // (ex: on se retourne plus vite que si on accélère dans la direction dans laquelle on va déjà)
        float velocity = targetVelocity - body.velocity.x;

        // Selon si on accélère ou renti, on peut régler un facteur d'accélération différent. 
        // Ces facteurs représentent en gros la vitesse à laquelle on accélère/descelere, en sachant que dans tous les cas avec AddForce ça peut pas être instant
        // Et si on mets des values trop grandes (genre 50 - 100) il se passe des trucs chelous ...
        // On applique ensuite ce facteur à notre vélocité
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? accelerationRate : deccelerationRate; // Si la target speed n'est pas nulle : accélération, sinon c'est qu'on ralenti
        velocity = velocity * accelRate;

        // On applique une puissance sur la vélocité pour smooth encore plus le truc 
        // Là j'avoue c'est de la magie noire, j'arrive pas à ressentir ce qu'il y a de différent entre genre : augmenter ce power, ou augmenter l'accélérationRate :/
        float direction = Mathf.Sign(velocity); // On garde la direction en mémoire
        velocity = Mathf.Abs(velocity); // Valeur absolue
        velocity = Mathf.Pow(velocity, velocityPower); // Applique une puissance
        velocity = velocity * direction; // Réapplique la direction

        // Debug.Log(velocity);

        body.AddForce(velocity * Vector2.right * Time.deltaTime, ForceMode2D.Impulse);


    }
    
    void Friction()
    {
        // Artificial Friction
        // Si on est au sol et qu'on essaie de s'arrêter (pas d'input), on va appliquer une anti-force de friction ou notre vélocité selon lequel des deux est le plus petit.
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
