using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManipulator : MonoBehaviour{

    [Header("Main")]
    public Camera HRC;
    public float VerticalSummonFix;
    public float rayLength = 100f;

    public GameObject Prefab1;

    [Header("Misc")]
    public string grabTag = "canGrab";
    private GameObject grabbedObject;
    private float grabbedDistance;

    private float ObjectMass = 0f;

    private Vector3 offset;
    public static bool isEKeyPressed = false;
    public float verticalOffset = 1.0f;

    [Header("KeyBinds")]
    public KeyCode grabKey = KeyCode.Mouse0;
    public KeyCode freezeKey = KeyCode.Mouse1;
    public KeyCode impulseKey = KeyCode.R;
    private float scroll;

    // Скрипт, который нужно отключать
    public MonoBehaviour scriptToDisable;
    // Скрипт, который нужно включать
    public MonoBehaviour scriptToEnable;

    private bool isGrabbing = false; // Добавлена переменная для отслеживания захвата

    void Start()
    {

    }

    void Update()
    {

        // Проверяем, нажата ли клавиша "O"
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Отключаем первый скрипт
            scriptToDisable.enabled = false;
            // Включаем второй скрипт
            scriptToEnable.enabled = true;
        }

        scroll = Input.GetAxis("Mouse ScrollWheel");
        Ray ray = HRC.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Input.GetKeyDown(grabKey)) { // Используем GetKeyDown вместо GetKey
            isGrabbing = true;
        }

        if (Input.GetKeyUp(grabKey)) { // Используем GetKeyUp для сброса захвата
            isGrabbing = false;
        }

        if (isGrabbing && grabbedObject == null){
            if (Physics.Raycast(ray, out hitData, rayLength)){
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                if (hitData.collider.CompareTag(grabTag)){
                    grabbedObject = hitData.collider.gameObject;
                    grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    ObjectMass = grabbedObject.GetComponent<Rigidbody>().mass;
                    grabbedDistance = hitData.distance;
                    offset = grabbedObject.transform.position - hitData.point;
                }
            }
        } else if (!isGrabbing && grabbedObject != null) {
            grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            grabbedObject = null;
            isEKeyPressed = false;
        }

        if (grabbedObject != null) {

            if (scroll > 0f){
                grabbedDistance += 1f;
            } else if (scroll < 0f) {
                grabbedDistance -= 1f; 
            }

            if (Input.GetKey(freezeKey)){
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                grabbedObject = null;
                isEKeyPressed = false;
            }

            if (Input.GetKey(impulseKey)) {
                Vector3 impulseDirection = HRC.transform.forward;
                grabbedObject.GetComponent<Rigidbody>().AddForce(impulseDirection*ObjectMass*10f, ForceMode.Impulse);
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
                grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                grabbedObject = null;
                isEKeyPressed = false;
            }

            if (Input.GetKey(KeyCode.E)) {
                isEKeyPressed = true;
                float rotationSpeed = 2.0f;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                grabbedObject.transform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.World);
                grabbedObject.transform.Rotate(Vector3.right, -mouseY * rotationSpeed, Space.World);
            } else {
                isEKeyPressed = false;
            }
        }

        if (grabbedObject != null) {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            Vector3 targetPosition = HRC.transform.position + HRC.transform.forward * grabbedDistance + offset;
            grabbedObject.transform.position = targetPosition;
        }



    }
}
