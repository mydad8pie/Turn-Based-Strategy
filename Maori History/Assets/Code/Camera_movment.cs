using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_movment : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        //gets the wasd input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //caclulaits the move direction
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Move the camera
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);



    }
}
