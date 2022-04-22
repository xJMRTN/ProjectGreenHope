using UnityEngine;

public class CameraController : MonoBehaviour
{
     [SerializeField] float horizontalSensitivity;
     [SerializeField] float verticalSensitivity;
     [SerializeField] float multiplier;
     [SerializeField] Transform cam;
     [SerializeField] Transform orientation;

    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation; 

    public bool locked = true;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        if(!locked) return;
        GetPlayerInput();
        MoveCamera();
    } 

    void GetPlayerInput(){
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * horizontalSensitivity * multiplier;
        xRotation -= mouseY * verticalSensitivity * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    void MoveCamera(){
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}