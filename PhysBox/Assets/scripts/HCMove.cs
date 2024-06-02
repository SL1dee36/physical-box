using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCMove : MonoBehaviour{
    public float sensX = 400f;
    public float sensY = 400f;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        if (GravityManipulator.isEKeyPressed) {
            return;
        }
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") *Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") *Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f,90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //transform.rotation = Quaternion.Euler(0, yRotation, 0);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        orientation.Rotate(Vector3.up * mouseX);

    }
}