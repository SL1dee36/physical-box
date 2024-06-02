using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    public float mouseSenisit = 100f;
    public Transform playerBody;
    float xRotation = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenisit * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenisit * Time.deltaTime;

        xRotation -= mouseY;
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}