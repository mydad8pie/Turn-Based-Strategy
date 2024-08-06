using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Camera mainCamera;
    public float delay = 0.1f;
    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(StartLookingAtCameraAfterDelay());
    }


    private IEnumerator StartLookingAtCameraAfterDelay()
    {
        yield return new WaitForSeconds(delay);


        while (true){
            if (mainCamera != null)
            {
                Vector3 directionToCamera = mainCamera.transform.position - transform.position;
                directionToCamera.x = 0;
            

                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
                transform.rotation = lookRotation;
    }
            yield return null;
        }
    }
}
