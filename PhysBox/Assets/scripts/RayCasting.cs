using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasting : MonoBehaviour
{

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

    private bool isGrabbing = false; // Добавлена переменная для отслеживания захвата

    private Rigidbody grabbedRigidbody; // Добавлен объект для хранения Rigidbody
    private Vector3 hitOffsetLocal; // Добавлен вектор для хранения смещения от точки попадания

    [Header("Physics Settings")]
    public float grabSpeed = 10f; // Скорость перемещения объекта

    // Скрипт, который нужно отключать
    public MonoBehaviour scriptToDisable;
    // Скрипт, который нужно включать
    public MonoBehaviour scriptToEnable;

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
                    grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>(); // Сохраняем Rigidbody
                    grabbedRigidbody.useGravity = false;
                    grabbedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                    ObjectMass = grabbedRigidbody.mass;
                    grabbedDistance = hitData.distance;
                    offset = grabbedObject.transform.position - hitData.point;
                    hitOffsetLocal = hitData.transform.InverseTransformVector(hitData.point - hitData.transform.position); // Сохраняем смещение
                }
            }
        } else if (!isGrabbing && grabbedObject != null) {
            grabbedRigidbody.useGravity = true;
            grabbedRigidbody.constraints = RigidbodyConstraints.None;
            grabbedObject = null;
            grabbedRigidbody = null;
            isEKeyPressed = false;
        }

        if (grabbedObject != null) {

            if (scroll > 0f){
                grabbedDistance += 1f;
            } else if (scroll < 0f) {
                grabbedDistance -= 1f; 
            }

            if (Input.GetKey(freezeKey)){
                grabbedRigidbody.useGravity = false;
                grabbedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                grabbedObject = null;
                grabbedRigidbody = null;
                isEKeyPressed = false;
            }

            if (Input.GetKey(impulseKey)) {
                Vector3 impulseDirection = HRC.transform.forward;
                grabbedRigidbody.AddForce(impulseDirection*ObjectMass*10f, ForceMode.Impulse);
                grabbedRigidbody.useGravity = true;
                grabbedRigidbody.constraints = RigidbodyConstraints.None;
                grabbedObject = null;
                grabbedRigidbody = null;
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

            if (grabbedObject != null) {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                // Перемещаем объект
                Vector3 targetPosition = HRC.transform.position + HRC.transform.forward * grabbedDistance + offset;
                // Устанавливаем скорость, чтобы объект плавно перемещался
                Vector3 velocity = Vector3.zero; // Объявление переменной velocity
                grabbedRigidbody.velocity = Vector3.SmoothDamp(grabbedRigidbody.velocity, (targetPosition - grabbedObject.transform.position).normalized * grabSpeed, ref velocity, 0.1f);
            }
        }
    }
}