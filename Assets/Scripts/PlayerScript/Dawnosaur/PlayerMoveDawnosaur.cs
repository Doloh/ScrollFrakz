using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveDawnosaur : MonoBehaviour
{
    // Déclare le playerData dans lequel on sélectionnera le scriptable object (hk, celeste, SMB...)
    public PlayerData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal"); // -1,1
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

        //Calculate difference between current velocity and desired velocity
        float speedDiff = targetSpeed - RB.velocity.x;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(speedDiff * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }
    #endregion
}
