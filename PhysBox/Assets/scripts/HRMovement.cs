using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRMovement : MonoBehaviour{
    
    [Header("Movement")]
    public float moveSpeed  = 25.0f;
    public float flySpeed   = 30.0f;
    public float groundDrag = 2.0f;
    public float gravity = 9.81f;
    public float jumpForce = 13.0f;

    public float airMultiplier = 1.2f;
    public float speedMultiplier = 1.2f;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode flyKey = KeyCode.V;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode qMenuKey = KeyCode.Q;
    public KeyCode runKey = KeyCode.LeftShift;
    bool flyState;

    [Header("Ground Check")]
    public float playerHeight = 4.0f;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Other")]
    public GameObject qMenu;
    public Transform orientation;
    public Camera HCR;
    bool qMenuState;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 flyDirection;

    Rigidbody rb;

    private void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        moveSpeed = moveSpeed*rb.mass;
        jumpForce = jumpForce*rb.mass;
        flySpeed = flySpeed*rb.mass;
        gravity = gravity*rb.mass*0.55f;

    }

    private void Update(){
        // ground chek
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f+0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();
        // handle drag
        if(grounded){
            rb.drag = groundDrag;
        } else if(!grounded && !flyState) {
            rb.drag = 0f;
            rb.AddForce(-transform.up * gravity, ForceMode.Force);
            //rb.position += new Vector3(0, -gravity*Time.deltaTime+airMultiplier, 0);
        } else {
            rb.drag = groundDrag;
        }
    }

    private void FixedUpdate(){
        
        if(flyState){
            Fly();
        } else {
            MovePlayer();
        }
    }

    private void MyInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //to jump
        if(Input.GetKeyDown(jumpKey) && grounded){
            Jump();
        } else if(Input.GetKeyDown(flyKey)){
            if(flyState){
                flyState = false;
                rb.useGravity = true;
            } else{
                flyState = true;
                rb.useGravity = false;
            }
        } else if(Input.GetKeyDown(qMenuKey)){
            if(qMenuState){
                GravityManipulator.isEKeyPressed = false;
                qMenuState = false;
                qMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }else{
                GravityManipulator.isEKeyPressed = true;
                qMenuState = true;
                qMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void MovePlayer(){

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * (Input.GetKey(runKey) ? (moveSpeed*speedMultiplier) : moveSpeed), ForceMode.Force);

    }

    private void SpeedControl(){

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if((flatVel.magnitude > moveSpeed) && grounded && !Input.GetKey(runKey)){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x,rb.velocity.y,limitedVel.z);
        }

    }

    private void Jump(){

        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void Fly(){

        flyDirection = (HCR.transform.forward * verticalInput + HCR.transform.right * horizontalInput).normalized;
        rb.AddForce(flyDirection * (Input.GetKey(runKey) ? (flySpeed*speedMultiplier) : flySpeed), ForceMode.Force);

        if(Input.GetKey(jumpKey)){
            rb.AddForce(transform.up * (Input.GetKey(runKey) ? (jumpForce*speedMultiplier) : jumpForce) * 2.0f, ForceMode.Force);
        } else if(Input.GetKey(crouchKey)){
            rb.AddForce(-transform.up * (Input.GetKey(runKey) ? (jumpForce*speedMultiplier) : jumpForce) * 2.0f, ForceMode.Force);
        }

    }
}
