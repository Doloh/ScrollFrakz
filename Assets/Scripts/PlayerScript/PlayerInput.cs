using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public float inputX;

    void Update()
    {
        // Version alternative de check si on a un inputX direct avec le key (si jamais on utilise pas le rawInput)
        /*
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            hasInputX = true;
        }
        else
        {
            hasInputX = false;
        }
        */

        inputX = Input.GetAxisRaw("Horizontal");

    }
}
