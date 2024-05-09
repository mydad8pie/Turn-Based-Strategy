using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_movment : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 10f;
    public float maxZoom = 20f;
    public float minZoom = 2f;

    // Update is called once per frame
    void Update()
    {
        // Getting the arrow key input
        float verticalInput = GetVerticalInput();
        float horizontalInput = GetHorizontalInput();

        // apply movement
        MoveCamera(verticalInput, horizontalInput);

        // Zooming
        // Scroll Wheel zoom control
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput != 0f)
        {
            ZoomCamera(scrollWheelInput);
        }
        //zooming with keyboard X and Z
        if (Input.GetKey(KeyCode.Z))
        {
            ZoomCamera(1f);

        }
        else if (Input.GetKey(KeyCode.X))
        {
            ZoomCamera(-1f);
        }

    }
    //
    float GetVerticalInput()
    {
        // Checking for arrow key presses for vertical movement
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            return Input.GetKey(KeyCode.UpArrow) ? 1f : -1f;
        }
        //vertical mouse input
        if (Input.mousePosition.y <= 0)
        {
            return -1f;
        }

        else if (Input.mousePosition.y >= Screen.height - 1)
        {
            return 1f;
        }
        return 0f;
    }
    float GetHorizontalInput()
    {
        // Checking for arrow key presses for horrazontal
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            return Input.GetKey(KeyCode.LeftArrow) ? -1f : 1f;
        }

        //horizontal mouse Input
        if (Input.mousePosition.x <= 0)
        {
            return -1f;
        }

        else if (Input.mousePosition.x >= Screen.width - 1)
        {
            return 1f;
        }
        return 0f;
    }

    void MoveCamera(float verticalInput, float horizontalInput)
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        //ingores the camera vertical vector
        forward.y = 0;
        forward.Normalize();

        //caclulaits the move direction
        Vector3 moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        // Move the camera
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

    }

    void ZoomCamera(float zoomAmount)
    {
        // updates camera postion with zoom
        Vector3 newPosition = transform.position + transform.forward * zoomAmount * zoomSpeed * Time.deltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, minZoom, maxZoom);

        // move the camera
        transform.position = newPosition;
    }

}