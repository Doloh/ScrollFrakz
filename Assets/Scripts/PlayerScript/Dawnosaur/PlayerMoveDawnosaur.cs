using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveDawnosaur : MonoBehaviour
{
    // D�clare le playerData dans lequel on s�lectionnera le scriptable object (hk, celeste, SMB...)
    public PlayerData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other scripts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public bool isGrounded;
    public bool IsJumping { get; private set; }
    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.39f, 0.03f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        IsFacingRight = true;
    }

    private void Update()
    {
        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal"); // -1,1

        // Si on a un input, on check dans quelle direction tourner le player
        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        // input jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            Jump();
        }
        #endregion


        #region COLLISION CHECKS
        //Ground Check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
        {
            isGrounded = true;
        }
        #endregion

        #region JUMP CHECKS
        // Si on tombe on est plus en train de sauter (sera utile plus tard)
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;
        }
        #endregion
    }

    private void FixedUpdate()
    {
        Run();
    }

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;

        #region Calculate AccelRate
        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop).
        float accelRate;
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        #endregion

        //Calculate difference between current velocity and desired velocity
        // �a induit 2 choses :
        // 1 - au plus on s'approche de notre vitesse max au moins on acc�l�re (�a smooth tout, boost de vitesse quand on tourne etc...)
        // 2 - on d�passera jamais notre vitesse max (on l'atteindra jamais vraiment non plus mais l'�cart sera invisible)
        // Autrement dit speedDiff tend vers 1/l'infini
        float speedDiff = targetSpeed - RB.velocity.x;
        float movement = speedDiff * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }
    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        if (!CanJump()) return;
        isGrounded = false;
        IsJumping = true;
        float force = Data.jumpHeight;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
    #endregion

    #region CHECK METHODS
    // On peut sauter si on est au sol et qu'on saute pas déjà
    private bool CanJump()
    {
        return isGrounded && !IsJumping;
    }
    // Check dans quelle direction on va
    public void CheckDirectionToFace(bool isMovingRight)
    {
        // Si on est pas tourné dans le sens de la direction, on turn
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        // Debug la box de ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion
}
