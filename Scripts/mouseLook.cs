using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class mouseLook : MonoBehaviour
{
    float mouseSensitivity = .4f;
    float xRotation = 0f;
    [SerializeField]
    Transform playerBody;
    public bool cameraLocked;
    void LateUpdate()
    {
        if(!cameraLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            float kbmouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity;
            float kbmouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity;

            xRotation -= kbmouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * kbmouseX);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class mouseLook : MonoBehaviour
{
    Vector3 mouseSensitivity = new Vector3(2000, -2000);
    float xRotation = 0f;
    float yRotation = 0f;
    [SerializeField]Transform playerBody;
    Vector2 velocity;
    Vector2 rotation;
    [SerializeField]Vector2 acceleration;
    float inputLagPeriod = 0.005f;
    float inputLagTimer;
    Vector2 lastInputEvent;

    Vector2 GetInput()
    {
        inputLagTimer += Time.deltaTime;
        float kbmouseX = Input.GetAxis("Mouse X");
        float kbmouseY = Input.GetAxis("Mouse Y");
        if((Mathf.Approximately(0, kbmouseX) && Mathf.Approximately(0, kbmouseY)) == false || inputLagTimer >= inputLagPeriod)
        {
            lastInputEvent = new Vector2(kbmouseX, kbmouseY);
            inputLagTimer = 0;
        }
        return lastInputEvent;
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector2 input = GetInput() * mouseSensitivity;
        velocity = new Vector2(
            Mathf.MoveTowards(velocity.x, input.x, acceleration.x * Time.deltaTime),
            Mathf.MoveTowards(velocity.y, input.y, acceleration.y * Time.deltaTime));
        rotation += velocity * Time.deltaTime;
        /*xRotation -= kbmouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    xRotation += velocity.x* Time.deltaTime;
    yRotation += velocity.y* Time.deltaTime;
    yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        transform.localEulerAngles = new Vector3(rotation.y, 0f, 0f);
    playerBody.localEulerAngles = new Vector3(0, rotation.x, 0);
}
}
*/
}
