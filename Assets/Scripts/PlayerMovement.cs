using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public World world;

    public bool isGrounded;

    public float speed = 10f;
    public float gravity  = -9.8f;
    public float jumpForce  = .5f;
    public float playerWidth = 0.15f;

    private float verticalMomentum = 0;
    private bool jumpRequest;
    private float horizontal;
    private float vertical;

    Vector3 velocity;
    Vector3 move;

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
        controller.Move(move * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (jumpRequest)
            Jump();
        transform.Translate(velocity, Space.World);
    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;
        
        move = transform.right * horizontal + transform.forward * vertical;

        if (move.z > 0 && front || move.z < 0 && back)
            move.z = 0;
        if (move.x > 0 && right || move.x < 0 && left)
            move.x = 0;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if (velocity.y < 0)
            velocity.y = CheckFallSpeed(velocity.y);  
        else if(velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);
    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }

    private void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
        Debug.Log("Is Jumping");
    }

    public float CheckFallSpeed(float fallSpeed)
    {
        if(world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + fallSpeed, transform.position.z - playerWidth) ||
           world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + fallSpeed, transform.position.z - playerWidth) ||
           world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + fallSpeed, transform.position.z + playerWidth) ||
           world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + fallSpeed, transform.position.z + playerWidth))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return fallSpeed;
        }
    }

    private float CheckUpSpeed(float upSpeed)
    {
        if(world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 2f, transform.position.z - playerWidth) ||
           world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 2f, transform.position.z - playerWidth) ||
           world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 2f, transform.position.z + playerWidth) ||
           world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 2f, transform.position.z + playerWidth))
        {
            return 0;
        }
        else
        {
            return upSpeed;
        }
    }

    public bool front
    {
        get
        {
            if (world.IsVoxelSolid(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
                world.IsVoxelSolid(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                return true;
            else
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (world.IsVoxelSolid(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
                world.IsVoxelSolid(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                return true;
            else
                return false;
        }
    }

    public bool left
    {
        get
        {
            if (world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
                world.IsVoxelSolid(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                return true;
            else
                return false;
        }
    }

    public bool right
    {
        get
        {
            if (world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
                world.IsVoxelSolid(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                return true;
            else
                return false;
        }
    }


}    

