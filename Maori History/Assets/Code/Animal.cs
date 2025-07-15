using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    // Reference to the Renderer component of the animal
    private Renderer animalRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Rotate the animal 90 degrees on the Z axis
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        // Get the Renderer component to change the color
        animalRenderer = GetComponent<Renderer>();

        if (animalRenderer != null)
        {
            // Set the animal color to pink
            animalRenderer.material.color = Color.magenta; // Pink color in Unity
        }
        else
        {
            Debug.LogError("Renderer component not found on the Animal object.");
        }
    }

    // Update is called once per frame
    
}
