using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 10.0f;
    public float gravity = -19.81f;
    public float jumpHeight = 2.52f;
    public float flySpeed = 10.0f;
    public float speedMultiply = 7.5f;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public GameObject Qmenu;

    Vector3 velocity;
    bool isGrounded;
    bool isFly;
    bool isQmenu;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (isQmenu) {
                Qmenu.SetActive(false);
                isQmenu = false;
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                Qmenu.SetActive(true);
                isQmenu = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2.0f;
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            isFly = !isFly;
            if (isFly) {
                velocity.y = 0; // сбрасываем вертикальную скорость при включении режима полёта
            }
        }

        if (isFly) {

            float x = 0;
            float z = 0;

            if (Input.GetKey(KeyCode.W)) {
                z = 1;
            } else if (Input.GetKey(KeyCode.S)) {
                z = -1;
            }

            if (Input.GetKey(KeyCode.A)) {
                x = -1;
            } else if (Input.GetKey(KeyCode.D)) {
                x = 1;
            }

            Vector3 move = transform.right * x + transform.forward * z;

            Vector3 flyDirection = (Camera.main.transform.forward * z + Camera.main.transform.right * x).normalized;
            if (x != 0 || z != 0) // двигаемся в направлении взгляда только при нажатии клавиши
            {
                if (Input.GetButton("LeftShift")) {
                    controller.Move(flyDirection * (flySpeed+speedMultiply) * Time.deltaTime);
                } else {
                controller.Move(flyDirection * flySpeed * Time.deltaTime);
                }
            }

            // Добавляем возможность подняться вверх или опуститься вниз
            if (Input.GetKey(KeyCode.Space)) // подняться вверх
            {
                controller.Move(Vector3.up * (Input.GetKey(KeyCode.LeftShift) ? (flySpeed + speedMultiply) : flySpeed) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftControl)) // опуститься вниз
            {
                controller.Move(Vector3.down * (Input.GetKey(KeyCode.LeftShift) ? (flySpeed + speedMultiply) : flySpeed) * Time.deltaTime);
            }
        } else {   

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (Input.GetButton("LeftShift")) {
                controller.Move(move * (speed+speedMultiply) * Time.deltaTime);
            } else {
            
            controller.Move(move * speed * Time.deltaTime);
            }
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

    }
}
