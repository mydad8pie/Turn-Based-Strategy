using UnityEngine;

public class Camera_movment : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private float zoomSpeed = 10f;
    [SerializeField]
    private float maxZoom = 40f;
    [SerializeField]
    private float minZoom = 2f;

    private float isometricView = 40f;
    private float topDownView = 60f;
    private bool firstSpacePress = true;
    private bool isRotated = false;


    void Update()
    {

        HandleRotationInput();
        HandleZoomInput();
        HandleMovementInput();

    }

    void HandleMovementInput(){
        float verticalInput = GetVerticalInput();
        float horizontalInput = GetHorizontalInput();

        // apply movement
        MoveCamera(verticalInput, horizontalInput);
    }
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

    void HandleZoomInput(){
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

        if (Input.GetKey(KeyCode.LeftShift)& Input.GetKey(KeyCode.UpArrow))
        {
            ZoomCamera(1f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) & Input.GetKey(KeyCode.DownArrow))
        {
            ZoomCamera(-1f);
        }
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
        // move the camera
        transform.position = newPosition; 
    }

    void HandleRotationInput(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotateCamera();
        }
    }

    public void RotateCamera()
    {
        Vector3 currentRotation = transform.eulerAngles;
        if (!isRotated)
        {
            isometricView = transform.position.y;
            if (!firstSpacePress){
                
                transform.position = new Vector3(transform.position.x, topDownView, transform.position.z);
            }
            maxZoom = 60f; 
            currentRotation.x = 90f;
            isRotated = true;
            firstSpacePress = false;
        }
        else
        {
            topDownView = transform.position.y;
            transform.position = new Vector3(transform.position.x, isometricView, transform.position.z);
            maxZoom = 40f;
            currentRotation.x = 45f;
            isRotated = false;
            
        }
        transform.eulerAngles = currentRotation;
    }
}