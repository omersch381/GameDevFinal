using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    public GameObject playerCamera;
    private CharacterController controller;
    private float speed = 0.5f;
    private float rx = 0, ry;
    private float angularSpeed = 4f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float dx, dz;

        // Mouse
        rx -= Input.GetAxis("Mouse Y") * angularSpeed / 4;
        playerCamera.transform.localEulerAngles = new Vector3(rx, 0, 0);

        ry = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * angularSpeed;
        transform.localEulerAngles = new Vector3(0,ry, 0);

        // Keyboard
        dx = Input.GetAxis("Horizontal") * speed;
        dz = Input.GetAxis("Vertical") * speed;
        
        Vector3 motion = new Vector3(dx,-1,dz);
        motion = transform.TransformDirection(motion);
        controller.Move(motion);
    }
}
