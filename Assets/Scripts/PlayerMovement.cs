using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public World world;
    public Transform cam;

    public bool isGrounded;

    public float speed = 10f;
    public float gravity  = -9.8f;
    public float jumpForce  = .5f;
    public float playerWidth = 0.15f;
    public int blastRadius = 4;

    private float verticalMomentum = 0;
    private bool jumpRequest;
    private float horizontal;
    private float vertical;
    private bool collision;
    Vector3 velocity;
    Vector3 move;
    Vector3 selectedBlock;

    public float checkIncrement = 0.1f;
    public float reach = 14f;

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
        SelectBlock();
        controller.Move(move * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (jumpRequest)
            Jump();
        transform.Translate(velocity, Space.World);
    }

    private void SelectBlock()
    {
        float step = checkIncrement;

        while(step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if(world.IsVoxelSolid(pos))
            {

                Vector3Int newPos = Vector3Int.FloorToInt(pos);
                selectedBlock = new Vector3(newPos.x, newPos.y, newPos.z);
                collision = true;

                return;
            }

            step += checkIncrement;
        }

        collision = false;
    }

    private void DestroyBlock(float offsetX, float offsetY)
    {
        int x = Mathf.FloorToInt(offsetX);
        int y = Mathf.FloorToInt(offsetY);
        Vector3 worldPos = new Vector3(selectedBlock.x + x, selectedBlock.y + y, selectedBlock.z);

        if(world.IsVoxelSolid(worldPos))
        {
            Vector3 voxelPos = world.GetVoxelChunkPosition(worldPos);
            world.GetChunk(worldPos).EditVoxel(voxelPos, 0);
        }
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

        if(collision)
        {
            if(Input.GetMouseButtonDown(0))
            {
                for (int x = 0; x < blastRadius; x++)
                {
                    for (int y = 0; y < blastRadius; y++)
                    {
                        if(Vector2.Distance(new Vector2(x-(blastRadius/2), 
                                            y-(blastRadius/2)), Vector2.zero)<=(blastRadius/3))
                        {
                            DestroyBlock(x - (blastRadius / 2), y - (blastRadius / 2));
                        }
                    }
                }           
            }
        }
    }

    private void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    public float CheckFallSpeed(float fallSpeed)
    {
        if(world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + fallSpeed, transform.position.z - playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + fallSpeed, transform.position.z - playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + fallSpeed, transform.position.z + playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + fallSpeed, transform.position.z + playerWidth)))
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
        if(world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f, transform.position.z - playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f, transform.position.z - playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f, transform.position.z + playerWidth)) ||
           world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f, transform.position.z + playerWidth)))
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
            if (world.IsVoxelSolid(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.IsVoxelSolid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)))
                return true;
            else
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (world.IsVoxelSolid(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.IsVoxelSolid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)))
                return true;
            else
                return false;
        }
    }

    public bool left
    {
        get
        {
            if (world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.IsVoxelSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)))
                return true;
            else
                return false;
        }
    }

    public bool right
    {
        get
        {
            if (world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.IsVoxelSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)))
                return true;
            else
                return false;
        }
    }


}    

