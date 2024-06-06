using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Camera_movment : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 10f;
    public float maxZoom = 40f;
    public float minZoom = 2f;
    private bool isRotated = false;
    private bool isPaused = false;


    //void Start()
    //{
    //    LockCursor();
    //}

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
            //TogglePause();
        //}

        //if (isPaused) return;
        
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotateCamera();
        }

        if (!isRotated && transform.position.y > 40f)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = 40f;
            transform.position = newPosition;
        }

    }
    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            UnlockCursor();
            Time.timeScale = 0f;//pasue game
        }
        else
        {
            LockCursor();
            Time.timeScale = 1f;//resume game
        }
    }
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
       
       // ingores the camera vertical vector when the camera is not rotated
        if (!isRotated)
        {
            forward.y = 0;
             forward.Normalize();
        }
        else
        {
            forward = transform.up;
        }
       

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

        if(newPosition.y == minZoom || newPosition.y == maxZoom)
        {
            newPosition.z = transform.position.z;
          
        }

        if(isRotated){
            maxZoom = 60f;
        }
        else
        {
            maxZoom = 40f;
        }
        

        // move the camera
        transform.position = newPosition;

        
        
    }

    public void RotateCamera()
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