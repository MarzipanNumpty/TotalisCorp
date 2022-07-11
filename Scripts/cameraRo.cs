using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRo : MonoBehaviour
{
    PlayerControls controls;
    bool moveToNextPos;
    [SerializeField] Transform[] camTransforms;
    int arrayPos;
    float speed = .2f;
    void Awake()
    {
        controls = new PlayerControls();
        controls.Movement.InventoryUp.performed += ctx => nextPos();
        controls.Movement.InventoryDown.performed += ctx => nextPosFast();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void nextPos()
    {
        moveToNextPos = true;
        speed = .2f;
    }
    void nextPosFast()
    {
        moveToNextPos = true;
        speed = .4f;
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(moveToNextPos)
        {
            var targetRot = Quaternion.LookRotation(camTransforms[arrayPos].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * 5 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRot) < 10)
            {
                transform.position = Vector3.MoveTowards(transform.position, camTransforms[arrayPos].position, speed);
            }
            //transform.LookAt(camTransforms[arrayPos]);
            if(Vector3.Distance(transform.position, camTransforms[arrayPos].position) < 1.0f)
            {
                moveToNextPos = false;
                arrayPos++;
            }
        }
    }
}
