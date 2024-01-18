using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class PlayerController : KinematicObject
{
    //platforming parameters
    public float jumpHeight;
    public float runSpeed;
    public float jumpDist;

    //calculated parameters
    float gravity;
    float jumpSpeed;

    //Variables
    bool isGrounded = false;
    // Start is called before the first frame update
    void Start()
    {
        gravity = 2.0f * runSpeed * runSpeed * jumpHeight / (jumpDist * jumpDist);
        jumpSpeed = jumpDist * gravity / runSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        float horizIn = Input.GetAxis("Horizontal");

        if(!isGrounded)
        {
            velocity += gravity * Time.deltaTime * Vector2.down;
        }

        velocity.x = runSpeed * horizIn;
        Move();
    }

    private void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Z))
        {
            velocity.y = jumpSpeed;
            isGrounded = false;
        }
    }
}
