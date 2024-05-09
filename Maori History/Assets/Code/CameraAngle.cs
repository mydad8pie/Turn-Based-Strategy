using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAngle : MonoBehaviour
{

    private bool isRotated = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        Vector3 currentRotation = transform.eulerAngles;
        if (!isRotated)
        {
            currentRotation.x = 90f;
            isRotated = true;
        }
        else
        {
            currentRotation.x = 45f;
            isRotated = false;
        }
        transform.eulerAngles = currentRotation;
    }
}
