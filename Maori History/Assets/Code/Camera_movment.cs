using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_movment : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        //gets the arrow key input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // getting the camrea forwards and right vectors
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
}
